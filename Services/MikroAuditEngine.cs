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

            // Fallback list of key Mikro ERP tables if JSON file not in runtime path
            if (_knownMikroTables.Count == 0)
            {
                var defaultTables = new[]
                {
                    "CARI_HESAPLAR", "CARI_HESAP_HAREKETLERI", "STOKLAR", "STOK_HAREKETLERI",
                    "SIPARISLER", "STOK_DEPO_DETAYLARI", "DEPOLAR", "PERSONELLER", "PROJELER",
                    "SORUMLULUK_MERKEZLERI", "CARI_HESAPLAR_YONETIM", "CARIDETAY", "STOKDETAY",
                    "BANKALAR_YONETIM", "KASALAR_YONETIM", "ODEME_EMIRLERI_YONETIM",
                    "STOK_SATIS_FIYAT_LISTELERI_YONETIM", "SIPARISLER_OZET", "vw_Cari_Hareket_Evrak_Isimleri",
                    "ODEME_EMIRLERI", "STOK_HAREKETLERI_GIRIS_CIKIS"
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
        /// Bu sayede hem [cha_iptal] = 0 hem cha_iptal = 0 aynı regex ile yakalanır.
        /// </summary>
        private string NormalizeSql(string sql)
        {
            return sql.Replace("[", "").Replace("]", "");
        }

        public MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            var report = new MikroComplianceReport();
            if (string.IsNullOrWhiteSpace(tsqlQuery)) return report;

            var rawSql = tsqlQuery.Trim();
            var sql = NormalizeSql(rawSql);
            var upperSql = sql.ToUpperInvariant();

            // 1. Is this a Mikro query?
            bool isMikroTenant = tenantName != null && tenantName.ToLowerInvariant().Contains("mikro");
            bool containsMikroTables = _knownMikroTables.Any(t => IsTableInSql(upperSql, t));

            if (!isMikroTenant && !containsMikroTables)
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
            // RULE 3: CANCELED & HIDDEN RECORDS FILTER - Penalty: -10 pts / -5 pts
            // -------------------------------------------------------------
            bool queriesMovements = IsTableInSql(upperSql, "CARI_HESAP_HAREKETLERI") || IsTableInSql(upperSql, "STOK_HAREKETLERI");
            if (queriesMovements)
            {
                bool hasIptalFilter = upperSql.Contains("CHA_IPTAL = 0") || upperSql.Contains("STH_IPTAL = 0");
                bool hasHiddenFilter = upperSql.Contains("CHA_HIDDEN = 0") || upperSql.Contains("STH_HIDDEN = 0");

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
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-03",
                        Title = "Kısmi Gizli/İptal Kayıt Filtresi",
                        PenaltyPoints = 5,
                        IssueDescription = "Hareket tablosunda iptal veya gizli kayıtlardan biri filtrelenmiş ancak diğeri unutulmuş (Örn: cha_iptal = 0 var ama cha_hidden = 0 eksik).",
                        V26RuleReference = "Mikro T-SQL Standartları: Hem iptal (iptal = 0) hem gizli (hidden = 0) kayıtlar birlikte filtrelenmelidir.",
                        RecommendedFix = "WHERE koşuluna `AND cha.cha_iptal = 0 AND cha.cha_hidden = 0` filtrelerinin her ikisini de ekleyin.",
                        Severity = "Warning"
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-03",
                        Title = "Eksik İptal/Gizli Kayıt Filtresi",
                        PenaltyPoints = 10,
                        IssueDescription = "Hareket tablosunda `cha_iptal = 0` ve `cha_hidden = 0` filtresi tamamen eksik. İptal edilmiş fişler toplama dahil edilebilir.",
                        V26RuleReference = "Mikro T-SQL Standartları: İptal edilmiş ve gizli kayıtlar sorgu sonuçlarına dahil edilmemelidir.",
                        RecommendedFix = "WHERE koşuluna `AND cha.cha_iptal = 0 AND cha.cha_hidden = 0` ekleyin.",
                        Severity = "Error"
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
                bool hasBracketedMsg = Regex.IsMatch(sql, @"\[msg_S_\d+(\\\w+)?\]", RegexOptions.IgnoreCase);
                if (hasBracketedMsg)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-06",
                        Title = "Yönetim View Braket Eşlemesi",
                        Description = "Yönetim view sorgularında [msg_S_....] alan isimleri v27 standardına uygun eşleşmiş."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-06",
                        Title = "Eksik Yönetim View Braket Eşlemesi",
                        PenaltyPoints = 10,
                        IssueDescription = "Yönetim view sorgusunda [msg_S_....] kolon isimleri kullanılnamış.",
                        V26RuleReference = "Madde 5: Yönetim view'larında `[msg_S_0078]`, `[msg_S_0957\\T]` braket alan isimleri kullanılmalıdır.",
                        RecommendedFix = "View kolon isimlerini şemadaki `[msg_S_....]` eşlemeleriyle değiştirin.",
                        Severity = "Error"
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 7: UPPER(COLUMN) LIKE UPPER(N'%...%') PATTERN - Penalty: -5 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("LIKE"))
            {
                bool hasUpperPattern = upperSql.Contains("UPPER(") && upperSql.Contains("LIKE UPPER(");
                if (hasUpperPattern)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-07",
                        Title = "Türkçe Karakter Uyumlu Arama Pattern'ı",
                        Description = "Metin aramalarında UPPER(kolon) LIKE UPPER(N'%...%') pattern'ı uygulanmış."
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
                        IssueDescription = "Arama sorgusunda `UPPER(kolon) LIKE UPPER(N'%...%')` pattern'ı kullanılmadığından Türkçe karakter arama kaçırma riski.",
                        V26RuleReference = "Madde 7: Sözel isim aramalarında `UPPER(kolon) LIKE UPPER(N'%...%')` kullanılmalıdır.",
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

            // Final Score Calculations & Grading
            score = Math.Max(0, Math.Min(100, score));
            report.Score = score;

            if (score >= 95)
            {
                report.Grade = "A+";
                report.GradeLabel = "Kusursuz Uyum (A+)";
                report.SummaryText = "T-SQL sorgusu Mikro v27 standartlarına ve veritabanı şemasına %100 kusursuz uyum sağlamaktadır.";
            }
            else if (score >= 85)
            {
                report.Grade = "A";
                report.GradeLabel = "Yüksek Uyum (A)";
                report.SummaryText = "T-SQL sorgusu Mikro v27 kurallarına yüksek oranda uymaktadır. Küçük iyileştirmeler mümkündür.";
            }
            else if (score >= 70)
            {
                report.Grade = "B";
                report.GradeLabel = "Orta Uyum (B)";
                report.SummaryText = "Sorguda bazı kritik v27 kuralları (filtre veya kur koruması) eksiktir. Düzeltme önerilir.";
            }
            else if (score >= 50)
            {
                report.Grade = "C";
                report.GradeLabel = "Zayıf Uyum (C)";
                report.SummaryText = "Sorguda önemli v27 standart ihlalleri tespit edilmiştir. İyileştirme yapılması şarttır.";
            }
            else
            {
                report.Grade = "F";
                report.GradeLabel = "Uyumsuz / Riskli (F)";
                report.SummaryText = "Sorgu Mikro v27 mimarisinden ciddi sapmalar göstermektedir ve performans/doğruluk riski taşımaktadır.";
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
