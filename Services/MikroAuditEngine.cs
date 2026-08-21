using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OzBiPortalCRM.Services
{
    public class MikroAuditEngine : IMikroAuditEngine
    {
        private readonly HashSet<string> _knownMikroTables = new(StringComparer.OrdinalIgnoreCase);

        public MikroAuditEngine()
        {
            LoadSchemaTables();
        }

        private void LoadSchemaTables()
        {
            try
            {
                var searchDirs = new[]
                {
                    Path.Combine(AppContext.BaseDirectory, "ERP", "Mikro", "json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "ERP", "Mikro", "json"),
                    Path.Combine(AppContext.BaseDirectory, "ERP", "Mikro"),
                    Path.Combine(Directory.GetCurrentDirectory(), "ERP", "Mikro"),
                    Path.Combine(AppContext.BaseDirectory, "Mikro"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Mikro")
                };

                string? foundSchemaFile = null;
                foreach (var dir in searchDirs)
                {
                    if (Directory.Exists(dir))
                    {
                        var schemaFiles = Directory.GetFiles(dir, "mikro_assistant_schema_*.json")
                            .OrderByDescending(f => f)
                            .ToList();

                        if (schemaFiles.Any())
                        {
                            foundSchemaFile = schemaFiles.First();
                            break;
                        }
                    }
                }

                if (foundSchemaFile != null && File.Exists(foundSchemaFile))
                {
                    var jsonContent = File.ReadAllText(foundSchemaFile);
                    using var doc = JsonDocument.Parse(jsonContent);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var elem in doc.RootElement.EnumerateArray())
                        {
                            if (elem.TryGetProperty("TABLE_NAME", out var tProp))
                            {
                                var tName = tProp.GetString();
                                if (!string.IsNullOrEmpty(tName)) _knownMikroTables.Add(tName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MikroAuditEngine LoadSchemaTables warning: " + ex.Message);
            }

            // Fallback list of key Mikro ERP tables (All 47 Canonical Mikro v27.1 Tables)
            if (_knownMikroTables.Count == 0)
            {
                var defaultTables = new[]
                {
                    "CARI_HESAPLAR", "CARI_HESAP_HAREKETLERI", "STOKLAR", "STOK_HAREKETLERI", "SIPARISLER",
                    "ODEME_EMIRLERI", "BANKALAR", "KASALAR", "DEPOLAR", "STOK_DEPO_DETAYLARI",
                    "STOK_SATIS_FIYAT_LISTELERI", "PERSONELLER", "VERILEN_TEKLIFLER", "STOK_HAREKETLERI_GIRIS_CIKIS",
                    "BANKALAR_YONETIM", "KASALAR_YONETIM", "CARI_HESAP_HAREKETLERI_CHOOSE_30", "CARI_HESAPLAR_YONETIM",
                    "STOK_HAREKETLERI_CHOOSE_32", "STOKDETAY", "CARI_HESAP_GRUPLARI", "CARI_HESAP_BOLGELERI",
                    "CARI_HESAP_ADRESLERI", "CARI_HESAP_YETKILILERI", "STOK_ANA_GRUPLARI", "STOK_ALT_GRUPLARI",
                    "STOK_MARKALARI", "STOK_KATEGORILERI", "BARKOD_TANIMLARI", "ALINAN_TEKLIFLER",
                    "ODEME_PLANLARI", "MUHASEBE_HESAP_PLANI", "MUHASEBE_FISLERI", "MUHASEBE_FIS_DETAYLARI",
                    "FIRMALAR", "DEPARTMANLAR", "SORUMLULUK_MERKEZLERI", "PROJELER", "HIZMET_HESAPLARI",
                    "MASRAF_HESAPLARI", "CARIDETAY", "ODEME_EMIRLERI_YONETIM", "STOK_DEPO_DETAYLARI_YONETIM",
                    "STOK_SATIS_FIYAT_LISTELERI_YONETIM", "vw_Cari_Hareket_Evrak_Isimleri", "STOK_HAREKETLERINE_MALIYET_YANSITMA",
                    "SIPARISLER_OZET"
                };
                foreach (var t in defaultTables) _knownMikroTables.Add(t);
            }
        }

        private bool IsTableInSql(string sqlUpper, string tableName)
        {
            var pattern = $@"(?<![A-Z0-9_]){Regex.Escape(tableName.ToUpperInvariant())}(?![A-Z0-9_])";
            return Regex.IsMatch(sqlUpper, pattern);
        }

        /// <summary>
        /// SQL Server braket notasyonunu ([kolon_adi]) sıyırarak normalize eder.
        /// </summary>
        private string NormalizeSql(string sql)
        {
            return sql.Replace("[", "").Replace("]", "");
        }

        public MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            return EvaluateQuery(tsqlQuery, userPrompt, tenantName, forceEvaluation: false);
        }

        public MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt, string? tenantName, bool forceEvaluation)
        {
            var report = new MikroComplianceReport();
            if (string.IsNullOrWhiteSpace(tsqlQuery)) return report;

            var rawSql = tsqlQuery.Trim();
            var sql = NormalizeSql(rawSql);
            var upperSql = sql.ToUpperInvariant();

            // 1. Is this a Mikro query?
            bool isMikroTenant = tenantName != null && tenantName.ToLowerInvariant().Contains("mikro");
            bool containsMikroTables = _knownMikroTables.Any(t => IsTableInSql(upperSql, t));

            if (!forceEvaluation && !isMikroTenant && !containsMikroTables)
            {
                report.IsMikroQuery = false;
                report.SummaryText = "Bu sorgu Mikro ERP veritabanı haricinde bir veri kaynağına aittir.";
                return report;
            }

            report.IsMikroQuery = true;
            int score = 100;

            // -------------------------------------------------------------
            // RULE 1: CARI CINS DISTINCTION (cha_cari_cins = 0/2/4) - Penalty: -15 pts
            // -------------------------------------------------------------
            if (IsTableInSql(upperSql, "CARI_HESAP_HAREKETLERI"))
            {
                bool hasCariCinsFilter = Regex.IsMatch(sql, @"cha_cari_cins\s*=", RegexOptions.IgnoreCase) ||
                                         Regex.IsMatch(sql, @"cha_cari_cins\s+IN", RegexOptions.IgnoreCase);
                if (hasCariCinsFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-01",
                        Title = "Ana Hesap Türü Ayrımı (cha_cari_cins)",
                        Description = "CARI_HESAP_HAREKETLERI sorgusunda Cari(0), Banka(2) veya Kasa(4) filtresi cha_cari_cins ile doğru uygulanmış."
                    });
                }
                else
                {
                    score -= 15;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-01",
                        Title = "Eksik Ana Hesap Türü Filtresi (cha_cari_cins)",
                        PenaltyPoints = 15,
                        IssueDescription = "CARI_HESAP_HAREKETLERI tablosuna cha_cari_cins filtresi eklenmemiş. Cari, banka ve kasa hareketleri birbirine karışabilir.",
                        V26RuleReference = "Madde 1: `cha_cari_cins = 0` (cari), `2` (banka), `4` (kasa) filtresi uygulanmalıdır.",
                        RecommendedFix = "WHERE koşuluna `AND cha.cha_cari_cins = 0` (veya ilgili hesap türü) ekleyin.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 2: TL CURRENCY RATE PROTECTION - Penalty: -15 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("CHA_MEBLAG") || upperSql.Contains("CHA_D_KUR") || upperSql.Contains("SIP_DOVIZ_KURU"))
            {
                bool hasRateProtection = Regex.IsMatch(sql, @"CASE\s+WHEN\s+.*d_cins.*THEN\s+1\.?0?\s+ELSE", RegexOptions.IgnoreCase) ||
                                          Regex.IsMatch(sql, @"CASE\s+WHEN\s+.*doviz.*THEN\s+1\.?0?\s+ELSE", RegexOptions.IgnoreCase) ||
                                          upperSql.Contains("CHA_D_CINS = 0 THEN 1.0 ELSE");

                if (hasRateProtection)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-02",
                        Title = "TL Döviz Kuru Koruması",
                        Description = "Döviz kuru çarpanlarında TL kayıtları için CASE WHEN doviz_cins = 0 THEN 1.0 ELSE kur END koruması uygulanmış."
                    });
                }
                else if (upperSql.Contains("CHA_MEBLAG * CHA_D_KUR") || upperSql.Contains("CHA_D_KUR"))
                {
                    score -= 15;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-02",
                        Title = "Güvensiz Kur Çarpanı (TL Sıfır Kur Riski)",
                        PenaltyPoints = 15,
                        IssueDescription = "Tutar kur ile çarpılırken TL satırlarında kurun 0 veya NULL olabilme riski için CASE WHEN koruması kullanılmamış.",
                        V26RuleReference = "Madde 2: `cha.cha_meblag * CASE WHEN cha.cha_d_cins = 0 THEN 1.0 ELSE cha.cha_d_kur END` yapısı kullanılmalıdır.",
                        RecommendedFix = "`cha_d_kur` çarpanını `CASE WHEN cha_d_cins = 0 THEN 1.0 ELSE cha_d_kur END` şeklinde güncelleyin.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 3: CANCELED & HIDDEN RECORDS FILTER - Penalty: -5 pts
            // -------------------------------------------------------------
            bool queriesMovements = IsTableInSql(upperSql, "CARI_HESAP_HAREKETLERI") || IsTableInSql(upperSql, "STOK_HAREKETLERI");
            bool isManagementViewOnly = (upperSql.Contains("_YONETIM") || upperSql.Contains("CARIDETAY") || upperSql.Contains("STOKDETAY")) && !queriesMovements;

            if (queriesMovements && !isManagementViewOnly)
            {
                bool hasIptalFilter = upperSql.Contains("CHA_IPTAL = 0") || upperSql.Contains("STH_IPTAL = 0") || upperSql.Contains("CHA_IPTAL=0") || upperSql.Contains("STH_IPTAL=0");
                bool hasHiddenFilter = upperSql.Contains("CHA_HIDDEN = 0") || upperSql.Contains("STH_HIDDEN = 0") || upperSql.Contains("CHA_HIDDEN=0") || upperSql.Contains("STH_HIDDEN=0");

                if (hasIptalFilter && hasHiddenFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-03",
                        Title = "İptal ve Gizli Kayıt Filtresi",
                        Description = "İptal edilmiş (iptal = 0) ve gizli kayıtlar (hidden = 0) tam olarak filtrelenmiş."
                    });
                }
                else if (hasIptalFilter || hasHiddenFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-03",
                        Title = "İptal/Gizli Kayıt Filtresi",
                        Description = "İptal kayıt filtresi başarıyla uygulandı."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-03",
                        Title = "Eksik İptal/Gizli Kayıt Filtresi",
                        PenaltyPoints = 5,
                        IssueDescription = "Hareket tablosunda `cha_iptal = 0` ve `cha_hidden = 0` filtresi eksik.",
                        V26RuleReference = "Mikro v27.1 Standartları: İptal edilmiş ve gizli kayıtlar sorgu sonuçlarına dahil edilmemelidir.",
                        RecommendedFix = "WHERE koşuluna `AND cha.cha_iptal = 0 AND cha.cha_hidden = 0` ekleyin.",
                        Severity = "Warning"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 4: GUID JOIN ACCURACY - Penalty: -15 pts
            // -------------------------------------------------------------
            if (IsTableInSql(upperSql, "STOK_HAREKETLERI") && IsTableInSql(upperSql, "SIPARISLER"))
            {
                bool hasGuidJoin = upperSql.Contains("SIP_GUID = STH_SIP_UID") || upperSql.Contains("STH_SIP_UID = SIP_GUID");
                if (hasGuidJoin)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-04",
                        Title = "Doğru GUID İlişkisi (Sipariş -> Stok Hareketi)",
                        Description = "Sipariş satırı ile stok hareketi sip_Guid = sth_sip_uid eşlemesiyle doğru bağlanmış."
                    });
                }
                else
                {
                    score -= 15;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-04",
                        Title = "Hatalı veya Eksik GUID İlişkisi",
                        PenaltyPoints = 15,
                        IssueDescription = "Sipariş ile stok hareketi arasındaki GUID eşleşmesi sth_sip_uid yerine hatalı alanla yapılmış veya eksik.",
                        V26RuleReference = "Madde 4: `sip.sip_Guid = sh.sth_sip_uid` eşleşmesi kullanılmalıdır.",
                        RecommendedFix = "JOIN koşulunu `ON sip.sip_Guid = sh.sth_sip_uid` olarak güncelleyin.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 6: MANAGEMENT VIEW BRACKET MAPPING - Penalty: -10 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("_YONETIM") || upperSql.Contains("CARIDETAY") || upperSql.Contains("STOKDETAY"))
            {
                bool hasBracketedMsg = Regex.IsMatch(rawSql, @"\[msg_S_\d+(\\\w+)?\]", RegexOptions.IgnoreCase) ||
                                       Regex.IsMatch(sql, @"msg_S_\d+", RegexOptions.IgnoreCase);
                if (hasBracketedMsg || upperSql.Contains("SELECT *") || upperSql.Contains("MSG_S_") || upperSql.Contains("CARIDETAY") || upperSql.Contains("STOKDETAY"))
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-06",
                        Title = "Yönetim View Alan Eşlemesi",
                        Description = "Yönetim view sorgularında alan isimleri ve view şeması Mikro v27.1 standardına uygun eşleşmiş."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-06",
                        Title = "Yönetim View Braket Eşlemesi Önerisi",
                        PenaltyPoints = 5,
                        IssueDescription = "Yönetim view sorgularında `[msg_S_....]` alan isimleri kullanılması önerilir.",
                        V26RuleReference = "Madde 5: Yönetim view'larında `[msg_S_0078]`, `[msg_S_0957\\T]` braket alan isimleri kullanılmalıdır.",
                        RecommendedFix = "View kolon isimlerini şemadaki `[msg_S_....]` eşlemeleriyle değiştirin.",
                        Severity = "Warning"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 7: UPPER(COLUMN) LIKE UPPER(N'%...%') PATTERN - Penalty: -5 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("LIKE") && (upperSql.Contains("CARI_UNVAN") || upperSql.Contains("STO_ISIM") || upperSql.Contains("UNVAN") || upperSql.Contains("MSG_S_0002") || upperSql.Contains("MSG_S_0201")))
            {
                bool hasUpperPattern = upperSql.Contains("UPPER(") || upperSql.Contains("COLLATE");
                if (hasUpperPattern || upperSql.Contains("N'"))
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-07",
                        Title = "Türkçe Karakter Uyumlu Arama Pattern'ı",
                        Description = "Metin aramalarında büyük/küçük harf ve Türkçe karakter koruması uygulanmış."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-07",
                        Title = "Hassas Olmayan Metin Araması",
                        PenaltyPoints = 5,
                        IssueDescription = "Arama sorgusunda `UPPER(kolon) LIKE UPPER(N'%...%')` pattern'ı önerilir.",
                        V26RuleReference = "Mikro v27.1 Madde 7: Sözel isim aramalarında `UPPER(kolon) LIKE UPPER(N'%...%')` kullanılmalıdır.",
                        RecommendedFix = "Arama filtresini `UPPER(car.cari_unvan1) LIKE UPPER(N'%' + @search + '%')` yapın.",
                        Severity = "Warning"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 8: DEEP PARSER - T-SQL GROUP BY / ORDER BY COMPATIBILITY - Penalty: -10 pts
            // -------------------------------------------------------------
            var groupByViolation = EvaluateGroupByOrderByCompatibility(sql);
            if (groupByViolation != null)
            {
                score -= groupByViolation.PenaltyPoints;
                report.Violations.Add(groupByViolation);
            }
            else if (upperSql.Contains("GROUP BY") && upperSql.Contains("ORDER BY"))
            {
                report.PassedChecks.Add(new MikroRuleCheck
                {
                    RuleId = "M-08",
                    Title = "T-SQL GROUP BY ve ORDER BY Uyumluluğu",
                    Description = "GROUP BY ve ORDER BY fıkraları T-SQL sentaks kurallarına ve kolon bağımlılıklarına tam uygun."
                });
            }

            // -------------------------------------------------------------
            // RULE 9: UNKNOWN / CHOOSE-VIEW TABLE DETECTION - Penalty: -8 pts
            // -------------------------------------------------------------
            var tableCheckViolation = EvaluateTableReferences(sql);
            if (tableCheckViolation != null)
            {
                score -= tableCheckViolation.PenaltyPoints;
                report.Violations.Add(tableCheckViolation);
            }
            else
            {
                report.PassedChecks.Add(new MikroRuleCheck
                {
                    RuleId = "M-09",
                    Title = "Veritabanı Tablo Doğrulaması",
                    Description = "Sorgudaki tüm tablolar Mikro ERP referans şemasında tanımlıdır."
                });
            }

            // -------------------------------------------------------------
            // RULE 10: DATE RANGE PATTERN (BETWEEN vs Open-End Date) - Penalty: -5 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("BETWEEN") && (upperSql.Contains("TAHI") || upperSql.Contains("TARIH") || upperSql.Contains("VADE") || upperSql.Contains("_TARIH") || upperSql.Contains("MSG_S_")))
            {
                bool usesUnsafeBetween = Regex.IsMatch(sql, @"BETWEEN\s+'\d{4}-\d{2}-\d{2}'\s+AND\s+'\d{4}-\d{2}-(28|29|30|31)'", RegexOptions.IgnoreCase);
                if (usesUnsafeBetween)
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-10",
                        Title = "Riskli Tarih Aralığı Filtresi (BETWEEN)",
                        PenaltyPoints = 5,
                        IssueDescription = "Tarih filtrelerinde ayın son günü BETWEEN ile kısıtlandığında saat/zaman bileşeni nedeniyle son gün verileri kaçabilir.",
                        V26RuleReference = "Madde 7: Ay ve çeyrek tarih aralıklarında `>= 'YYYY-MM-01' AND < 'YYYY-MM+1-01'` açık aralık deseni kullanılmalıdır.",
                        RecommendedFix = "`BETWEEN '2026-08-01' AND '2026-08-31'` yerine `>= '2026-08-01' AND < '2026-09-01'` yazın.",
                        Severity = "Warning"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 11: CHECK / PROMISSORY NOTE RULES (sck_borclu OR search & Tahsil / Kalan Filter) - Penalty: -10 pts
            // -------------------------------------------------------------
            if (IsTableInSql(upperSql, "ODEME_EMIRLERI") || upperSql.Contains("ODEME_EMIRLERI_YONETIM"))
            {
                bool isNameSearch = upperSql.Contains("CARI_UNVAN1") || upperSql.Contains("MSG_S_0201") || upperSql.Contains("CARI_KOD");
                bool hasBorcluSearch = upperSql.Contains("SCK_BORCLU") || upperSql.Contains("MSG_S_1407");

                if (isNameSearch && !hasBorcluSearch && upperSql.Contains("LIKE"))
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-11",
                        Title = "Eksik Borçlu / Keşideci Taraması (sck_borclu)",
                        PenaltyPoints = 10,
                        IssueDescription = "Çek/senet müşteri adı aramasında yalnızca sahip cari aranmış, asıl borçlu/keşideci (sck_borclu / [msg_S_1407]) OR ile taranmamış. Cirolu müşteri çekleri sonuçlarda kaçabilir.",
                        V26RuleReference = "Madde 8: Çek aramalarında cari unvanı (CARI_HESAPLAR / [msg_S_0201]) ve asıl keşideci (sck_borclu / [msg_S_1407]) OR ile birlikte taranmalıdır.",
                        RecommendedFix = "WHERE koşulunu `(UPPER(c.cari_unvan1) LIKE UPPER(N'%...%') OR UPPER(oe.sck_borclu) LIKE UPPER(N'%...%'))` şeklinde güncelleyin.",
                        Severity = "Error"
                    });
                }
                else if (isNameSearch && hasBorcluSearch)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-11",
                        Title = "Doğru Keşideci & Cari Arama Pattern'ı",
                        Description = "Müşteri çeki aramasında sahip cari ve asıl keşideci (sck_borclu / [msg_S_1407]) OR ile birlikte taranmış."
                    });
                }

                // Check for Tahsilat vs Kalan > 0 anti-pattern
                bool isTahsilPrompt = !string.IsNullOrEmpty(userPrompt) && 
                                     (userPrompt.ToLowerInvariant().Contains("tahsil") || 
                                      userPrompt.ToLowerInvariant().Contains("ödenen") ||
                                      userPrompt.ToLowerInvariant().Contains("ödenmiş"));

                bool hasKalanFilter = Regex.IsMatch(sql, @"(sck_tutar\s*-\s*sck_odenen|msg_S_0301\\T)\s*>\s*0", RegexOptions.IgnoreCase) ||
                                      upperSql.Contains("KALAN_TUTAR > 0");

                if (isTahsilPrompt && hasKalanFilter)
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-11",
                        Title = "Tahsil Edilen Çeklerde Hatalı Açık Bakiye Filtresi",
                        PenaltyPoints = 10,
                        IssueDescription = "Tahsil olan/ödenen çek istendiğinde `kalan > 0` filtresi konulmuş. Tahsil edilmiş (kalanı 0 olan) çekler sorguda elenmektedir.",
                        V26RuleReference = "Madde 8: Tahsil olan/ödenen çek istendiğinde `sck_odenen > 0 OR sck_sonpoz = 10` uygulanmalı; `kalan > 0` filtresi konulmamalıdır.",
                        RecommendedFix = "`kalan > 0` filtresini kaldırıp `sck_odenen > 0 OR sck_sonpoz = 10` (view: `[msg_S_0238\\T] > 0 OR [msg_S_0297] = N'Ödendi'`) ekleyin.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE M-12: cha_evrak_tip INVOICE TYPE CHECK - Penalty: -10 pts
            // v27.2: Satış faturası = cha_evrak_tip = 63, Alış = 0
            // Fatura türü belirtilmezse alış + satış karışır
            // -------------------------------------------------------------
            if (IsTableInSql(upperSql, "CARI_HESAP_HAREKETLERI") &&
                (upperSql.Contains("SUM(") || upperSql.Contains("CHA_MEBLAG")) &&
                (upperSql.Contains("FATURA") || upperSql.Contains("CIRO") ||
                 (userPrompt != null && (userPrompt.ToLowerInvariant().Contains("fatura") ||
                                          userPrompt.ToLowerInvariant().Contains("ciro") ||
                                          userPrompt.ToLowerInvariant().Contains("satış") ||
                                          userPrompt.ToLowerInvariant().Contains("alış")))))
            {
                bool hasEvrakTipFilter = Regex.IsMatch(sql, @"cha_evrak_tip\s*=\s*(0|63)", RegexOptions.IgnoreCase) ||
                                          Regex.IsMatch(sql, @"cha_evrak_tip\s+IN", RegexOptions.IgnoreCase);
                if (hasEvrakTipFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-12",
                        Title = "Fatura Türü Ayrımı (cha_evrak_tip)",
                        Description = "Cari hareket sorgusunda fatura türü (cha_evrak_tip = 63: Satış, 0: Alış) doğru filtrelenmiş."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-12",
                        Title = "Eksik Fatura Türü Filtresi (cha_evrak_tip)",
                        PenaltyPoints = 10,
                        IssueDescription = "CARI_HESAP_HAREKETLERI tablosunda fatura ciro/bakiye hesaplanırken cha_evrak_tip filtresi eksik. Alış ve satış faturaları birbirine karışabilir.",
                        V26RuleReference = "Mikro v27.2 Madde 2: Satış faturası `cha_evrak_tip = 63 AND cha_tip = 0`, Alış faturası `cha_evrak_tip = 0 AND cha_tip = 1` olarak filtrelenmelidir.",
                        RecommendedFix = "Satış cirosu için: `AND cha.cha_evrak_tip = 63 AND cha.cha_tip = 0` ekleyin.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE M-13: sck_sonpoz PROBLEMATIC PORTFOLIO CHECK - Penalty: -5 pts
            // v27.2: Karşılıksız/protestolu çek analizi sck_sonpoz IN (5,6) gerektirir
            // -------------------------------------------------------------
            if ((IsTableInSql(upperSql, "ODEME_EMIRLERI") || upperSql.Contains("ODEME_EMIRLERI_YONETIM")) &&
                userPrompt != null &&
                (userPrompt.ToLowerInvariant().Contains("karşılıksız") ||
                 userPrompt.ToLowerInvariant().Contains("protestolu") ||
                 userPrompt.ToLowerInvariant().Contains("sorunlu") ||
                 userPrompt.ToLowerInvariant().Contains("riskli çek") ||
                 userPrompt.ToLowerInvariant().Contains("riskli portföy")))
            {
                bool hasSonpozFilter = Regex.IsMatch(sql, @"sck_sonpoz\s+IN\s*\(\s*5\s*,\s*6\s*\)", RegexOptions.IgnoreCase) ||
                                       Regex.IsMatch(sql, @"sck_sonpoz\s*=\s*(5|6)", RegexOptions.IgnoreCase) ||
                                       Regex.IsMatch(sql, @"msg_S_0297.*(?:Karşılıksız|Protestolu)", RegexOptions.IgnoreCase);
                if (hasSonpozFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-13",
                        Title = "Sorunlu Portföy Filtresi (sck_sonpoz)",
                        Description = "Karşılıksız/protestolu çek analizinde sck_sonpoz IN (5, 6) filtresi doğru uygulanmış."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-13",
                        Title = "Eksik Sorunlu Portföy Filtresi (sck_sonpoz)",
                        PenaltyPoints = 5,
                        IssueDescription = "Karşılıksız/protestolu çek analizi istendiğinde sck_sonpoz IN (5, 6) filtresi uygulanmamış. Tüm çekler sonuca dahil olur.",
                        V26RuleReference = "Mikro v27.2 Madde 7: Sorunlu portföy `sck_sonpoz IN (5, 6)` (5: Karşılıksız, 6: Protestolu) ile filtrelenir.",
                        RecommendedFix = "WHERE koşuluna `AND oe.sck_sonpoz IN (5, 6)` ekleyin.",
                        Severity = "Warning"
                    });
                }
            }
            // -------------------------------------------------------------
            // RULE M-14: cha_vade TRY_CONVERT PROTECTION - Penalty: -10 pts
            // v27.2 Madde 8: cha_vade INT→DATE dönüşümünde TRY_CONVERT zorunlu
            // CAST(cha_vade AS date) hata verir çünkü cha_vade INTEGER tipinde
            // -------------------------------------------------------------
            if (upperSql.Contains("CHA_VADE"))
            {
                bool usesTryConvert = Regex.IsMatch(sql, @"TRY_CONVERT\s*\(\s*date\s*,\s*CONVERT\s*\(\s*(?:char|varchar)\s*\(\s*8\s*\)\s*,\s*\[?(?:\w+\.)?cha_vade", RegexOptions.IgnoreCase);
                bool usesUnsafeCast = Regex.IsMatch(sql, @"CAST\s*\(\s*\[?(?:\w+\.)?cha_vade\]?\s+AS\s+date\s*\)", RegexOptions.IgnoreCase);

                if (usesTryConvert)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-14",
                        Title = "Güvenli Vade Tarihi Dönüşümü (TRY_CONVERT)",
                        Description = "cha_vade INTEGER alanı TRY_CONVERT(date, CONVERT(char(8), cha_vade), 112) ile güvenli şekilde dönüştürülmüş."
                    });
                }
                else if (usesUnsafeCast)
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-14",
                        Title = "Güvensiz Vade Tarihi Dönüşümü (CAST)",
                        PenaltyPoints = 10,
                        IssueDescription = "cha_vade INTEGER alanı CAST(...AS date) ile doğrudan dönüştürülmüş. cha_vade INT olduğundan bu dönüşüm hata verir.",
                        V26RuleReference = "Mikro v27.2 Madde 8: `TRY_CONVERT(date, CONVERT(char(8), cha_vade), 112)` kullanılmalıdır.",
                        RecommendedFix = "`CAST(cha_vade AS date)` yerine `TRY_CONVERT(date, CONVERT(char(8), cha.cha_vade), 112)` kullanın.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE M-15: sth_normal_iade RETURN DISTINCTION - Penalty: -5 pts
            // v27.2: Satış cirosu hesabında iade satırları düşülmeli
            // sth_normal_iade = 1 ise tutar negatif uygulanmalı
            // -------------------------------------------------------------
            if (IsTableInSql(upperSql, "STOK_HAREKETLERI") &&
                (upperSql.Contains("SUM(") || upperSql.Contains("STH_TUTAR")) &&
                userPrompt != null &&
                (userPrompt.ToLowerInvariant().Contains("ciro") ||
                 userPrompt.ToLowerInvariant().Contains("satış gelir") ||
                 userPrompt.ToLowerInvariant().Contains("net satış")))
            {
                bool hasIadeFilter = Regex.IsMatch(sql, @"sth_normal_iade\s*=\s*[01]", RegexOptions.IgnoreCase) ||
                                     Regex.IsMatch(sql, @"CASE\s+WHEN\s+.*sth_normal_iade\s*=\s*1\s+THEN\s+-", RegexOptions.IgnoreCase) ||
                                     Regex.IsMatch(sql, @"CASE\s+WHEN\s+.*normal_iade.*THEN\s+-", RegexOptions.IgnoreCase);
                if (hasIadeFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-15",
                        Title = "Satış İade Ayrımı (sth_normal_iade)",
                        Description = "Satış cirosu hesabında iade satırları (sth_normal_iade = 1) doğru şekilde negatif olarak düşülmüş."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-15",
                        Title = "Eksik Satış İade Ayrımı (sth_normal_iade)",
                        PenaltyPoints = 5,
                        IssueDescription = "Satış cirosu hesaplanırken sth_normal_iade = 1 (iade) satırları ayrıştırılmamış. İadeler ciroyu şişirebilir.",
                        V26RuleReference = "Mikro v27.2 Agent Prompt: `sth_normal_iade = 1` iade satırları cirodan düşülmelidir.",
                        RecommendedFix = "Ciro formülünü `SUM(CASE WHEN sth_normal_iade = 1 THEN -sth_tutar ELSE sth_tutar END)` olarak güncelleyin.",
                        Severity = "Warning"
                    });
                }
            }

            // Final Score Calculations & Grading
            score = Math.Max(0, Math.Min(100, score));
            report.Score = score;

            if (score >= 95)
            {
                report.Grade = "A+";
                report.GradeLabel = "Kusursuz Uyum (A+)";
                report.SummaryText = "T-SQL sorgusu Mikro v27.1 standartlarına ve veritabanı şemasına %100 kusursuz uyum sağlamaktadır.";
            }
            else if (score >= 85)
            {
                report.Grade = "A";
                report.GradeLabel = "Yüksek Uyum (A)";
                report.SummaryText = "T-SQL sorgusu Mikro v27.1 kurallarına yüksek oranda uymaktadır. Küçük iyileştirmeler mümkündür.";
            }
            else if (score >= 70)
            {
                report.Grade = "B";
                report.GradeLabel = "Orta Uyum (B)";
                report.SummaryText = "Sorguda bazı kritik v27.1 kuralları (filtre veya kur koruması) eksiktir. Düzeltme önerilir.";
            }
            else if (score >= 50)
            {
                report.Grade = "C";
                report.GradeLabel = "Zayıf Uyum (C)";
                report.SummaryText = "Sorguda önemli v27.1 standart ihlalleri tespit edilmiştir. İyileştirme yapılması şarttır.";
            }
            else
            {
                report.Grade = "F";
                report.GradeLabel = "Uyumsuz / Riskli (F)";
                report.SummaryText = "Sorgu Mikro v27.1 mimarisinden ciddi sapmalar göstermektedir ve performans/doğruluk riski taşımaktadır.";
            }

            return report;
        }

        /// <summary>
        /// Derin T-SQL Parser: GROUP BY fıkrası olan sorgularda ORDER BY içinde geçen kolonların
        /// aggregate veya GROUP BY içerisinde olup olmadığını kontrol eder.
        /// </summary>
        private MikroRuleViolation? EvaluateGroupByOrderByCompatibility(string sql)
        {
            try
            {
                var upperSql = sql.ToUpperInvariant();
                int groupByIdx = upperSql.IndexOf("GROUP BY", StringComparison.Ordinal);
                int orderByIdx = upperSql.IndexOf("ORDER BY", StringComparison.Ordinal);

                if (groupByIdx == -1 || orderByIdx == -1 || orderByIdx < groupByIdx)
                    return null;

                // Extract GROUP BY clause
                string groupByClause = sql.Substring(groupByIdx + 8, orderByIdx - (groupByIdx + 8)).Trim();
                
                // Extract ORDER BY clause (up to end or HAVING / FOR OFFSET if any)
                int endOrderIdx = sql.Length;
                var nextKeywords = new[] { "HAVING", "OPTION", "OFFSET" };
                foreach (var kw in nextKeywords)
                {
                    int kIdx = upperSql.IndexOf(kw, orderByIdx, StringComparison.Ordinal);
                    if (kIdx != -1 && kIdx < endOrderIdx) endOrderIdx = kIdx;
                }
                string orderByClause = sql.Substring(orderByIdx + 8, endOrderIdx - (orderByIdx + 8)).Trim();

                // Tokenize GROUP BY expressions
                var groupByTokens = TokenizeExpressions(groupByClause);

                // Tokenize ORDER BY expressions
                var orderByTokens = TokenizeExpressions(orderByClause);

                // Extract column identifiers from ORDER BY that are NOT inside aggregate functions
                foreach (var orderExpr in orderByTokens)
                {
                    var nonAggColumns = ExtractNonAggregateColumns(orderExpr);
                    foreach (var col in nonAggColumns)
                    {
                        // Check if numeric column reference (e.g. ORDER BY 1, 2)
                        if (int.TryParse(col, out _)) continue;

                        // Check if column or exact expression exists in GROUP BY tokens
                        bool isGrouped = groupByTokens.Any(g => 
                            g.Equals(col, StringComparison.OrdinalIgnoreCase) || 
                            g.Contains(col, StringComparison.OrdinalIgnoreCase) ||
                            col.Contains(g, StringComparison.OrdinalIgnoreCase));

                        // Check if col is a SELECT alias
                        // Normalized SQL has no brackets, so check for AS alias pattern
                        bool isAlias = Regex.IsMatch(sql, $@"\bAS\s+{Regex.Escape(col)}\b", RegexOptions.IgnoreCase);

                        if (!isGrouped && !isAlias)
                        {
                            return new MikroRuleViolation
                            {
                                RuleId = "M-08",
                                Title = "Geçersiz T-SQL ORDER BY / GROUP BY Kolon Bağımlılığı",
                                PenaltyPoints = 10,
                                IssueDescription = $"ORDER BY fıkrasındaki '{col}' kolonu ne bir aggregate (SUM/COUNT vb.) fonksiyon içinde ne de GROUP BY fıkrasında yer almaktadır. SQL Server sorguyu çalıştırmayacaktır.",
                                V26RuleReference = "T-SQL Standartları: GROUP BY içeren sorgularda ORDER BY kolonu GROUP BY listesinde veya aggregate fonksiyon içinde olmak zorundadır.",
                                RecommendedFix = $"ORDER BY '{col}' yerine sütun indeksi (Örn: `ORDER BY 1, 2`) veya GROUP BY'daki CASE ifadesinin alias'ını kullanın.",
                                Severity = "Error"
                            };
                        }
                    }
                }
            }
            catch
            {
                // Parser fallback
            }

            return null;
        }

        private List<string> TokenizeExpressions(string clause)
        {
            var result = new List<string>();
            int depth = 0;
            int start = 0;

            for (int i = 0; i < clause.Length; i++)
            {
                char c = clause[i];
                if (c == '(') depth++;
                else if (c == ')') depth--;
                else if (c == ',' && depth == 0)
                {
                    result.Add(clause.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }
            if (start < clause.Length)
            {
                result.Add(clause.Substring(start).Trim());
            }

            return result.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        }

        private List<string> ExtractNonAggregateColumns(string expression)
        {
            var cols = new List<string>();
            var upperExpr = expression.ToUpperInvariant();

            // If the expression starts with an aggregate function, ignore column checks inside it
            var aggFunctions = new[] { "SUM(", "COUNT(", "MAX(", "MIN(", "AVG(", "STDEV(" };
            if (aggFunctions.Any(f => upperExpr.StartsWith(f)))
            {
                return cols;
            }

            // Extract table.column or column identifiers
            var matches = Regex.Matches(expression, @"(?:\[?([A-Za-z0-9_]+)\]?\.)?\[?([A-Za-z0-9_]+)\]?");
            var sqlKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SELECT", "FROM", "WHERE", "AND", "OR", "CASE", "WHEN", "THEN", "ELSE", "END",
                "ASC", "DESC", "NULL", "NOT", "IN", "IS", "LIKE", "YEAR", "MONTH", "DAY", "DATEFROMPARTS"
            };

            foreach (Match m in matches)
            {
                var colName = m.Groups[2].Value;
                if (!sqlKeywords.Contains(colName) && !int.TryParse(colName, out _))
                {
                    cols.Add(m.Value.Trim());
                }
            }

            return cols.Distinct().ToList();
        }

        /// <summary>
        /// SQL içerisindeki FROM ve JOIN tablolarını ayıklayıp şema ile karşılaştırır.
        /// </summary>
        private MikroRuleViolation? EvaluateTableReferences(string sql)
        {
            try
            {
                // Find all defined CTE names in the query (WITH cte_name AS ... or , cte_name AS ...)
                var definedCtes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var cteMatches = Regex.Matches(sql, @"(?:WITH|,)\s*\[?([A-Za-z0-9_]+)\]?\s+AS\s*\(", RegexOptions.IgnoreCase);
                foreach (Match cm in cteMatches)
                {
                    if (cm.Groups[1].Success)
                    {
                        definedCtes.Add(cm.Groups[1].Value);
                    }
                }

                var matches = Regex.Matches(sql, @"\b(?:FROM|JOIN)\s+\[?([A-Za-z0-9_]+)\]?", RegexOptions.IgnoreCase);
                var unknownTables = new List<string>();
                var variantTables = new List<string>();

                foreach (Match m in matches)
                {
                    string tbl = m.Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(tbl)) continue;

                    // If it's a defined CTE, skip
                    if (definedCtes.Contains(tbl)) continue;

                    bool isKnown = _knownMikroTables.Any(t => t.Equals(tbl, StringComparison.OrdinalIgnoreCase));
                    if (!isKnown)
                    {
                        // Check if it is a known variant/choose-view (e.g. _CHOOSE_30, _GIRIS_CIKIS, _YONETIM)
                        bool isVariant = tbl.Contains("_CHOOSE_", StringComparison.OrdinalIgnoreCase) ||
                                         tbl.Contains("_GIRIS_CIKIS", StringComparison.OrdinalIgnoreCase) ||
                                         tbl.Contains("vw_", StringComparison.OrdinalIgnoreCase);

                        if (isVariant)
                        {
                            variantTables.Add(tbl);
                        }
                        else
                        {
                            unknownTables.Add(tbl);
                        }
                    }
                }

                if (unknownTables.Count > 0)
                {
                    return new MikroRuleViolation
                    {
                        RuleId = "M-09",
                        Title = "Şemada Bulunmayan Tablo Referansı",
                        PenaltyPoints = 8,
                        IssueDescription = $"Sorguda kullanılan '{string.Join(", ", unknownTables.Distinct())}' tablosu/tabloları Mikro ERP şemasında bulunmamaktadır.",
                        V26RuleReference = "Mikro T-SQL Şema Kuralları: Yalnızca şemada tanımlı olan yetkili veritabanı nesneleri kullanılmalıdır.",
                        RecommendedFix = "Şemadaki geçerli tablo veya view adlarını kullanın.",
                        Severity = "Error"
                    };
                }

                if (variantTables.Count > 0)
                {
                    return new MikroRuleViolation
                    {
                        RuleId = "M-09",
                        Title = "Şema Harici Mikro View / Choose-View Kullanımı",
                        PenaltyPoints = 0, // No score penalty for known Mikro view variants
                        IssueDescription = $"Sorguda Mikro ERP'ye özel '{string.Join(", ", variantTables.Distinct())}' view yapısı kullanılmıştır. Şemada doğrudan olmasa da Mikro veritabanında mevcut bilinen bir varyanttır.",
                        V26RuleReference = "Mikro ERP Özel Nesneleri: Ek özelleştirilmiş view referansları.",
                        RecommendedFix = "Gerekirse şema tanımını güncelleyin.",
                        Severity = "Warning"
                    };
                }
            }
            catch
            {
                // Fallback
            }

            return null;
        }
    }
}
