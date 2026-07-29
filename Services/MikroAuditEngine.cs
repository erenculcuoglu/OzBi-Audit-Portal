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
                var schemaPath = Path.Combine(AppContext.BaseDirectory, "Mikro", "mikro_assistant_schema_20260722.json");
                if (!File.Exists(schemaPath))
                {
                    schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "Mikro", "mikro_assistant_schema_20260722.json");
                }

                if (File.Exists(schemaPath))
                {
                    var jsonContent = File.ReadAllText(schemaPath);
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
                    "STOK_SATIS_FIYAT_LISTELERI_YONETIM", "SIPARISLER_OZET", "vw_Cari_Hareket_Evrak_Isimleri"
                };
                foreach (var t in defaultTables) _knownMikroTables.Add(t);
            }
        }

        public MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            var report = new MikroComplianceReport();
            if (string.IsNullOrWhiteSpace(tsqlQuery)) return report;

            var sql = tsqlQuery.Trim();
            var upperSql = sql.ToUpperInvariant();

            // 1. Is this a Mikro query?
            bool isMikroTenant = tenantName != null && tenantName.ToLowerInvariant().Contains("mikro");
            bool containsMikroTables = _knownMikroTables.Any(t => upperSql.Contains(t.ToUpperInvariant()));

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
            if (upperSql.Contains("CARI_HESAP_HAREKETLERI"))
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
                        RecommendedFix = "WHERE koşuluna `AND cha.cha_cari_cins = 0` (veya ilgili hesap türü) ekleyin."
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
                        RecommendedFix = "`cha_d_kur` çarpanını `CASE WHEN cha_d_cins = 0 THEN 1.0 ELSE cha_d_kur END` şeklinde güncelleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 3: CANCELED & HIDDEN RECORDS FILTER (cha_iptal=0, cari_iptal=0) - Penalty: -10 pts
            // -------------------------------------------------------------
            bool queriesMovements = upperSql.Contains("CARI_HESAP_HAREKETLERI") || upperSql.Contains("STOK_HAREKETLERI");
            if (queriesMovements)
            {
                bool hasIptalFilter = upperSql.Contains("CHA_IPTAL = 0") || upperSql.Contains("STH_IPTAL = 0") || upperSql.Contains("CHA_HIDDEN = 0");
                if (hasIptalFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-03",
                        Title = "İptal/Gizli Kayıt Filtresi",
                        Description = "İptal edilmiş (iptal = 1) veya gizli kayıtlar filtrelenmiş."
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
                        IssueDescription = "Hareket tablosunda `cha_iptal = 0` veya `cha_hidden = 0` filtresi eksik. İptal edilmiş fişler toplama dahil edilebilir.",
                        V26RuleReference = "Mikro T-SQL Standartları: İptal edilmiş kayıtlar sorgu sonuçlarına dahil edilmemelidir.",
                        RecommendedFix = "WHERE koşuluna `AND cha.cha_iptal = 0 AND cha.cha_hidden = 0` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 4: GUID JOIN ACCURACY - Penalty: -15 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("STOK_HAREKETLERI") && upperSql.Contains("SIPARIS"))
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
                        RecommendedFix = "JOIN koşulunu `ON sip.sip_Guid = sh.sth_sip_uid` olarak güncelleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE 5: DEFAULT TOP 10 LIMIT RULE - Penalty: -10 pts
            // -------------------------------------------------------------
            bool isListingQuery = upperSql.Contains("ORDER BY") || upperSql.Contains("SELECT ");
            bool specifiesNumberInPrompt = userPrompt != null && Regex.IsMatch(userPrompt, @"\b(1|2|3|4|5|6|7|8|9|10|15|20|50|100)\b");

            if (isListingQuery && !specifiesNumberInPrompt)
            {
                bool hasTopClause = Regex.IsMatch(sql, @"SELECT\s+TOP\s+\d+", RegexOptions.IgnoreCase);
                if (hasTopClause)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "M-05",
                        Title = "Varsayılan TOP Limiti",
                        Description = "Kullanıcı sayı belirtmediğinde performans için varsayılan TOP 10 sınırı uygulanmış."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "M-05",
                        Title = "Eksik TOP Sınırı (Sınırsız Sonuç Riski)",
                        PenaltyPoints = 10,
                        IssueDescription = "Listeleme sorgusunda TOP sınırı kullanılmamış. Devasa veri kümesi çekme ve performans kaybı riski.",
                        V26RuleReference = "Madde 7: Listeleme sorgularında kullanıcı sayı belirtmemişse varsayılan TOP 10 kullanılmalıdır.",
                        RecommendedFix = "Sorgu başına `SELECT TOP (10) ...` ifadesini ekleyin."
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
                        Description = "Yönetim view sorgularında [msg_S_....] alan isimleri v26 standardına uygun eşleşmiş."
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
                        IssueDescription = "Yönetim view sorgusunda [msg_S_....] kolon isimleri kullanılmamış.",
                        V26RuleReference = "Madde 5: Yönetim view'larında `[msg_S_0078]`, `[msg_S_0957\\T]` braket alan isimleri kullanılmalıdır.",
                        RecommendedFix = "View kolon isimlerini şemadaki `[msg_S_....]` eşlemeleriyle değiştirin."
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
                        RecommendedFix = "Arama filtresini `UPPER(car.cari_unvan1) LIKE UPPER(N'%' + @search + '%')` yapın."
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
                report.SummaryText = "T-SQL sorgusu Mikro v26 standartlarına ve veritabanı şemasına %100 kusursuz uyum sağlamaktadır.";
            }
            else if (score >= 85)
            {
                report.Grade = "A";
                report.GradeLabel = "Yüksek Uyum (A)";
                report.SummaryText = "T-SQL sorgusu Mikro v26 kurallarına yüksek oranda uymaktadır. Küçük iyileştirmeler mümkündür.";
            }
            else if (score >= 70)
            {
                report.Grade = "B";
                report.GradeLabel = "Orta Uyum (B)";
                report.SummaryText = "Sorguda bazı kritik v26 kuralları (filtre veya kur koruması) eksiktir. Düzeltme önerilir.";
            }
            else if (score >= 50)
            {
                report.Grade = "C";
                report.GradeLabel = "Zayıf Uyum (C)";
                report.SummaryText = "Sorguda önemli v26 standart ihlalleri tespit edilmiştir. İyileştirme yapılması şarttır.";
            }
            else
            {
                report.Grade = "F";
                report.GradeLabel = "Uyumsuz / Riskli (F)";
                report.SummaryText = "Sorgu Mikro v26 mimarisinden ciddi sapmalar göstermektedir ve performans/doğruluk riski taşımaktadır.";
            }

            return report;
        }
    }
}
