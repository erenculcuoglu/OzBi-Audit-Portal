using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OzBiPortalCRM.Services
{
    public class LogoAuditEvaluator
    {
        private readonly HashSet<string> _knownLogoTables = new(StringComparer.OrdinalIgnoreCase);

        public LogoAuditEvaluator()
        {
            LoadLogoSchemaTables();
        }

        private void LoadLogoSchemaTables()
        {
            try
            {
                var schemaFileName = "logo_assistant_schema_v7.json";
                var schemaPath = Path.Combine(AppContext.BaseDirectory, "Logo", schemaFileName);
                if (!File.Exists(schemaPath))
                {
                    schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", schemaFileName);
                }

                if (!File.Exists(schemaPath))
                {
                    schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "logo_assistant_schema.json");
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
                                if (!string.IsNullOrEmpty(tName))
                                {
                                    _knownLogoTables.Add(tName);
                                    // Add un-prefixed table name too (e.g. CLCARD, STLINE)
                                    var cleanedName = Regex.Replace(tName, @"^LG_(XXX|\d+)_(\d+_)?", "", RegexOptions.IgnoreCase);
                                    if (!string.IsNullOrEmpty(cleanedName)) _knownLogoTables.Add(cleanedName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LogoAuditEvaluator schema load warning: " + ex.Message);
            }

            if (_knownLogoTables.Count == 0)
            {
                var defaultTables = new[] { "CLCARD", "ITEMS", "STLINE", "INVOICE", "CLFLINE", "BANKACC", "BNFLINE", "KSCARD", "KSLINES", "ORFICHE", "ORFLINE", "CSCARD" };
                foreach (var t in defaultTables) _knownLogoTables.Add(t);
            }
        }

        public MikroComplianceReport Evaluate(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            var report = new MikroComplianceReport
            {
                IsMikroQuery = true
            };

            if (string.IsNullOrWhiteSpace(tsqlQuery)) return report;

            var sql = tsqlQuery.Trim();
            var upperSql = sql.ToUpperInvariant();
            int score = 100;

            // -------------------------------------------------------------
            // RULE L-01: CANCELLED = 0 FILTER - Penalty: -15 pts
            // -------------------------------------------------------------
            bool queriesTransactions = upperSql.Contains("CLFLINE") || upperSql.Contains("STLINE") ||
                                       upperSql.Contains("INVOICE") || upperSql.Contains("STFICHE") ||
                                       upperSql.Contains("ORFLINE") || upperSql.Contains("BNFLINE") ||
                                       upperSql.Contains("KSLINES") || upperSql.Contains("CSCARD");
            if (queriesTransactions)
            {
                bool hasCancelledFilter = Regex.IsMatch(sql, @"CANCELLED\s*=\s*0", RegexOptions.IgnoreCase);
                if (hasCancelledFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-01",
                        Title = "İptal Kayıt Filtresi (CANCELLED = 0)",
                        Description = "Logo ERP v7 hareket tablolarında iptal kayıtlar CANCELLED = 0 ile süzülmüş."
                    });
                }
                else
                {
                    score -= 15;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-01",
                        Title = "Eksik İptal Kayıt Filtresi (CANCELLED = 0)",
                        PenaltyPoints = 15,
                        IssueDescription = "Hareket tablosunda CANCELLED = 0 filtresi eksik. İptal edilmiş fişler/faturalar toplama dahil olabilir.",
                        V26RuleReference = "Logo v7 Standardı: Hareket tablolarında `CANCELLED = 0` zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND CANCELLED = 0` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-02: ACTIVE = 0 FILTER ON CARDS - Penalty: -10 pts
            // -------------------------------------------------------------
            bool queriesCards = upperSql.Contains("CLCARD") || upperSql.Contains("ITEMS") ||
                                upperSql.Contains("BNCARD") || upperSql.Contains("BANKACC") ||
                                upperSql.Contains("KSCARD") || upperSql.Contains("SRVCARD");
            if (queriesCards)
            {
                bool hasActiveFilter = Regex.IsMatch(sql, @"ACTIVE\s*=\s*0", RegexOptions.IgnoreCase);
                if (hasActiveFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-02",
                        Title = "Aktif Kart Filtresi (ACTIVE = 0)",
                        Description = "Kart tablolarında pasif kartları süzmek için ACTIVE = 0 filtresi kullanılmış."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-02",
                        Title = "Eksik Aktif Kart Filtresi (ACTIVE = 0)",
                        PenaltyPoints = 10,
                        IssueDescription = "Kart tablosunda ACTIVE = 0 (0 = Aktif) filtresi eksik. Pasif kartlar sonuçlara karışabilir.",
                        V26RuleReference = "Logo v7 Standardı: Kart tablolarında `ACTIVE = 0` filtresi zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND ACTIVE = 0` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-03: WITH (NOLOCK) CLAUSE - Penalty: -10 pts
            // -------------------------------------------------------------
            bool hasNoLock = upperSql.Contains("WITH (NOLOCK)") || upperSql.Contains("WITH(NOLOCK)");
            if (hasNoLock)
            {
                report.PassedChecks.Add(new MikroRuleCheck
                {
                    RuleId = "L-03",
                    Title = "WITH (NOLOCK) Kullanımı",
                    Description = "Veritabanı kilitlenmelerini önlemek için WITH (NOLOCK) kullanılmış."
                });
            }
            else
            {
                score -= 10;
                report.Violations.Add(new MikroRuleViolation
                {
                    RuleId = "L-03",
                    Title = "Eksik WITH (NOLOCK) İfadesi",
                    PenaltyPoints = 10,
                    IssueDescription = "Logo T-SQL sorgusunda WITH (NOLOCK) kullanılmamış. Canlı veritabanında kilitlenme riski.",
                    V26RuleReference = "Logo v7 Standardı: Tüm tablo ve JOIN'lerde `WITH (NOLOCK)` zorunludur.",
                    RecommendedFix = "FROM ve JOIN tablolarının yanına `WITH (NOLOCK)` ekleyin."
                });
            }

            // -------------------------------------------------------------
            // RULE L-04: TRCODE & LINETYPE DISTINCTION - Penalty: -15 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("STLINE"))
            {
                bool hasLinetypeFilter = Regex.IsMatch(sql, @"LINETYPE\s*=", RegexOptions.IgnoreCase) || Regex.IsMatch(sql, @"LINETYPE\s+IN", RegexOptions.IgnoreCase);
                if (hasLinetypeFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-04",
                        Title = "Satır Türü Ayrımı (LINETYPE)",
                        Description = "STLINE tablosunda LINETYPE = 0 (Malzeme) veya LINETYPE = 4 (Hizmet) ayrımı uygulanmış."
                    });
                }
                else
                {
                    score -= 15;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-04",
                        Title = "Eksik Satır Türü Filtresi (LINETYPE)",
                        PenaltyPoints = 15,
                        IssueDescription = "STLINE tablosunda LINETYPE filtresi eksik. İskonto ve promosyon satırları stok miktarlarına karışabilir.",
                        V26RuleReference = "Logo v7 Standardı: Malzeme hareketleri için `LINETYPE = 0` filtresi zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND LINETYPE = 0` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-05: DEFAULT TOP LIMIT (TOP 50 in Logo v7) - Penalty: -10 pts
            // -------------------------------------------------------------
            bool isListingQuery = upperSql.Contains("ORDER BY") || upperSql.Contains("SELECT ");
            bool specifiesNumber = userPrompt != null && Regex.IsMatch(userPrompt, @"\b(1|2|3|4|5|6|7|8|9|10|15|20|50|100)\b");

            if (isListingQuery && !specifiesNumber)
            {
                bool hasTopClause = Regex.IsMatch(sql, @"SELECT\s+TOP\s+\d+", RegexOptions.IgnoreCase);
                if (hasTopClause)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-05",
                        Title = "Varsayılan TOP Limiti",
                        Description = "Listeleme sorgularında varsayılan TOP 50 sınırı uygulanmış."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-05",
                        Title = "Eksik TOP Sınırı (TOP 50)",
                        PenaltyPoints = 10,
                        IssueDescription = "Sorguda TOP sınırı kullanılmamış. Performans kaybı riski.",
                        V26RuleReference = "Logo v7 Standardı: Listeleme sorgularında varsayılan `TOP 50` zorunludur.",
                        RecommendedFix = "Sorgu başına `SELECT TOP 50 ...` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-06: TURKISH UPPER SEARCH PATTERN - Penalty: -5 pts
            // -------------------------------------------------------------
            if (upperSql.Contains("LIKE"))
            {
                bool hasUpperPattern = upperSql.Contains("UPPER(") && upperSql.Contains("LIKE UPPER(");
                if (hasUpperPattern)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-06",
                        Title = "Türkçe Karakter Uyumlu Arama Pattern'ı",
                        Description = "Metin aramalarında UPPER(...) LIKE UPPER(N'%...%') kullanılmış."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-06",
                        Title = "Hassas Olmayan Metin Araması",
                        PenaltyPoints = 5,
                        IssueDescription = "Metin aramasında `UPPER(kolon) LIKE UPPER(N'%...%')` kullanılmadığından Türkçe karakter kaçırma riski.",
                        V26RuleReference = "Logo v7 Standardı: Metin aramaları `WHERE UPPER(C.[DEFINITION_]) LIKE UPPER(N'%...%')` şeklinde olmalıdır.",
                        RecommendedFix = "Arama filtresini `UPPER(C.[DEFINITION_]) LIKE UPPER(N'%' + @search + '%')` olarak güncelleyin."
                    });
                }
            }

            // Final score calculations
            score = Math.Max(0, Math.Min(100, score));
            report.Score = score;

            if (score >= 95)
            {
                report.Grade = "A+";
                report.GradeLabel = "Logo v7 Kusursuz Uyum (A+)";
                report.SummaryText = "T-SQL sorgusu Logo ERP v7 standartlarına ve şemasına %100 kusursuz uyum sağlamaktadır.";
            }
            else if (score >= 85)
            {
                report.Grade = "A";
                report.GradeLabel = "Logo v7 Yüksek Uyum (A)";
                report.SummaryText = "T-SQL sorgusu Logo ERP v7 kurallarına yüksek oranda uymaktadır.";
            }
            else if (score >= 70)
            {
                report.Grade = "B";
                report.GradeLabel = "Logo v7 Orta Uyum (B)";
                report.SummaryText = "Logo sorgusunda bazı v7 standart filtreleri (CANCELLED, ACTIVE veya NOLOCK) eksiktir.";
            }
            else if (score >= 50)
            {
                report.Grade = "C";
                report.GradeLabel = "Logo v7 Zayıf Uyum (C)";
                report.SummaryText = "Sorguda önemli Logo ERP v7 standart ihlalleri tespit edilmiştir.";
            }
            else
            {
                report.Grade = "F";
                report.GradeLabel = "Logo v7 Uyumsuz (F)";
                report.SummaryText = "Sorgu Logo ERP v7 mimarisinden ciddi sapmalar göstermektedir.";
            }

            return report;
        }
    }
}
