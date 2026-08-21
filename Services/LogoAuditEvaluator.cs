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
                var searchDirs = new[]
                {
                    Path.Combine(AppContext.BaseDirectory, "ERP", "Logo", "json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "ERP", "Logo", "json"),
                    Path.Combine(AppContext.BaseDirectory, "ERP", "Logo"),
                    Path.Combine(Directory.GetCurrentDirectory(), "ERP", "Logo"),
                    Path.Combine(AppContext.BaseDirectory, "Logo", "json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Logo", "json"),
                    Path.Combine(AppContext.BaseDirectory, "Logo"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Logo")
                };

                string? schemaPath = null;
                foreach (var dir in searchDirs)
                {
                    if (Directory.Exists(dir))
                    {
                        var schemaFiles = Directory.GetFiles(dir, "logo_assistant_schema_*.json")
                            .OrderByDescending(f => f)
                            .ToList();

                        if (schemaFiles.Any())
                        {
                            schemaPath = schemaFiles.First();
                            break;
                        }
                    }
                }

                if (schemaPath != null && File.Exists(schemaPath))
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
                var defaultTables = new[]
                {
                    "CLCARD", "ITEMS", "STLINE", "INVOICE", "CLFLINE", "BANKACC", "BNFLINE",
                    "KSCARD", "KSLINES", "ORFICHE", "ORFLINE", "CSCARD", "PAYTRANS", "CSROLL",
                    "CSTRANS", "EMFLINE", "SRVCARD", "EMUHACC", "PROJECT", "PAYPLANS", "PAYLINES",
                    "PRCLIST", "SLSCLREL", "BNFICHE", "CLFICHE", "CLTOTFIL", "EMCENTER", "SPECODES",
                    "MARK", "UNITSETF", "UNITSETL", "UNITBARCODE"
                };
                foreach (var t in defaultTables) _knownLogoTables.Add(t);
            }
        }

        private bool IsTableInSql(string sqlUpper, string tableName)
        {
            var pattern = $@"(?<![A-Z0-9_]){Regex.Escape(tableName.ToUpperInvariant())}(?![A-Z0-9_])";
            return Regex.IsMatch(sqlUpper, pattern);
        }

        public MikroComplianceReport Evaluate(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            return Evaluate(tsqlQuery, userPrompt, tenantName, forceEvaluation: false);
        }

        public MikroComplianceReport Evaluate(string tsqlQuery, string? userPrompt, string? tenantName, bool forceEvaluation)
        {
            var report = new MikroComplianceReport();

            if (string.IsNullOrWhiteSpace(tsqlQuery)) return report;

            var sql = tsqlQuery.Trim();
            var upperSql = sql.ToUpperInvariant();

            bool isLogoTenant = tenantName != null && tenantName.ToLowerInvariant().Contains("logo");
            bool containsLogoTables = _knownLogoTables.Any(t => IsTableInSql(upperSql, t)) || upperSql.Contains("LG_");

            if (!forceEvaluation && !isLogoTenant && !containsLogoTables)
            {
                report.IsMikroQuery = false;
                report.SummaryText = "Bu sorgu Logo ERP veritabanı haricinde bir veri kaynağına aittir.";
                return report;
            }

            report.IsMikroQuery = true;
            int score = 100;

            // -------------------------------------------------------------
            // RULE L-01: CANCELLED = 0 FILTER - Penalty: -15 pts (Logo ERP v8.0)
            // Movement tables: CLFLINE, STLINE, STFICHE, INVOICE, ORFLINE, BNFLINE, KSLINES, PAYTRANS, CSROLL, CSTRANS, EMFLINE, CSCARD, CLFICHE, BNFICHE
            // -------------------------------------------------------------
            bool queriesTransactions = upperSql.Contains("CLFLINE") || upperSql.Contains("STLINE") ||
                                       upperSql.Contains("INVOICE") || upperSql.Contains("STFICHE") ||
                                       upperSql.Contains("ORFLINE") || upperSql.Contains("BNFLINE") ||
                                       upperSql.Contains("KSLINES") || upperSql.Contains("CSCARD") ||
                                       upperSql.Contains("PAYTRANS") || upperSql.Contains("CSROLL") ||
                                       upperSql.Contains("CSTRANS") || upperSql.Contains("EMFLINE") ||
                                       upperSql.Contains("CLFICHE") || upperSql.Contains("BNFICHE");
            if (queriesTransactions)
            {
                bool hasCancelledFilter = Regex.IsMatch(sql, @"CANCELLED\s*=\s*0", RegexOptions.IgnoreCase);
                if (hasCancelledFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-01",
                        Title = "İptal Kayıt Filtresi (CANCELLED = 0)",
                        Description = "Logo ERP v8.0 hareket tablolarında iptal kayıtlar CANCELLED = 0 ile süzülmüş."
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
                        V26RuleReference = "Logo ERP v8.0 Standardı: Hareket tablolarında `CANCELLED = 0` zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND CANCELLED = 0` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-02: ACTIVE = 0 FILTER ON CARDS - Penalty: -10 pts (Logo ERP v8.0)
            // Card tables: CLCARD, ITEMS, BNCARD, BANKACC, KSCARD, SRVCARD, EMUHACC, PROJECT, PRCLIST, PAYPLANS
            // -------------------------------------------------------------
            bool queriesCards = upperSql.Contains("CLCARD") || upperSql.Contains("ITEMS") ||
                                upperSql.Contains("BNCARD") || upperSql.Contains("BANKACC") ||
                                upperSql.Contains("KSCARD") || upperSql.Contains("SRVCARD") ||
                                upperSql.Contains("EMUHACC") || upperSql.Contains("PROJECT") ||
                                upperSql.Contains("PRCLIST") || upperSql.Contains("PAYPLANS");
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
                        V26RuleReference = "Logo ERP v8.0 Standardı: Kart tablolarında `ACTIVE = 0` filtresi zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND ACTIVE = 0` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-03: WITH (NOLOCK) CLAUSE - Penalty: -10 pts (Logo ERP v8.0)
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
                    V26RuleReference = "Logo ERP v8.0 Standardı: Tüm tablo ve JOIN'lerde `WITH (NOLOCK)` zorunludur.",
                    RecommendedFix = "FROM ve JOIN tablolarının yanına `WITH (NOLOCK)` ekleyin."
                });
            }

            // -------------------------------------------------------------
            // RULE L-04: TRCODE & LINETYPE DISTINCTION - Penalty: -15 pts (Logo ERP v8.0)
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
                        Description = "STLINE tablosunda LINETYPE IN (0, 1) (Malzeme) ayrımı uygulanmış."
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
                        V26RuleReference = "Logo ERP v8.0 Standardı: Malzeme hareketleri için `LINETYPE IN (0, 1)` filtresi zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND LINETYPE IN (0, 1)` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-05: DEFAULT TOP LIMIT (TOP 50) - Penalty: -10 pts (Logo ERP v8.0)
            // Rationalized: Aggregate & GROUP BY queries do NOT require TOP 50
            // -------------------------------------------------------------
            bool hasGroupBy = upperSql.Contains("GROUP BY");
            bool isScalarAggregate = Regex.IsMatch(upperSql, @"SELECT\s+(?:SUM|COUNT|AVG|MIN|MAX)\s*\(") && !upperSql.Contains("ORDER BY");
            bool isAggregateOrSummary = hasGroupBy || isScalarAggregate;

            bool hasTopClause = Regex.IsMatch(sql, @"SELECT\s+TOP\s+\d+", RegexOptions.IgnoreCase);
            bool specifiesNumber = userPrompt != null && Regex.IsMatch(userPrompt, @"\b(1|2|3|4|5|6|7|8|9|10|15|20|50|100)\b");

            if (isAggregateOrSummary)
            {
                report.PassedChecks.Add(new MikroRuleCheck
                {
                    RuleId = "L-05",
                    Title = "Özet / Kümülasyon Sorgusu (TOP 50 N/A)",
                    Description = "Sorgu GROUP BY veya toplulaştırma (SUM/COUNT) içerdiğinden varsayılan satır listeleme limiti (TOP 50) aranmamıştır."
                });
            }
            else if (!specifiesNumber)
            {
                if (hasTopClause)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-05",
                        Title = "Varsayılan TOP Limiti (TOP 50)",
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
                        IssueDescription = "Listeleme sorgusunda TOP sınırı kullanılmamış. Performans kaybı riski.",
                        V26RuleReference = "Logo ERP v8.0 Standardı: Listeleme sorgularında varsayılan `TOP 50` zorunludur.",
                        RecommendedFix = "Sorgu başına `SELECT TOP 50 ...` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-06: TURKISH UPPER SEARCH PATTERN - Penalty: -5 pts (Logo ERP v8.0)
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
                        V26RuleReference = "Logo ERP v8.0 Standardı: Metin aramaları `WHERE UPPER(C.[DEFINITION_]) LIKE UPPER(N'%...%')` şeklinde olmalıdır.",
                        RecommendedFix = "Arama filtresini `UPPER(C.[DEFINITION_]) LIKE UPPER(N'%' + @search + '%')` olarak güncelleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-07: KSLINES SIGN DIRECTION CHECK - Penalty: -10 pts (Logo ERP v8.0 Madde 8)
            // SIGN = 0 = Giriş (Borç), SIGN = 1 = Çıkış (Alacak)
            // -------------------------------------------------------------
            if (upperSql.Contains("KSLINES"))
            {
                bool hasReversedSign = Regex.IsMatch(sql, @"SIGN\s*=\s*1\s+THEN.*(?:GIRIS|CASH_IN|NAKIT_GIRIS|GELEN|TAHSILAT)", RegexOptions.IgnoreCase) ||
                                       Regex.IsMatch(sql, @"SIGN\s*=\s*0\s+THEN.*(?:CIKIS|CASH_OUT|NAKIT_CIKIS|GIDEN|ODEME)", RegexOptions.IgnoreCase);
                bool hasCorrectSign = Regex.IsMatch(sql, @"SIGN\s*=\s*0\s+THEN.*(?:GIRIS|CASH_IN|NAKIT_GIRIS|GELEN|TAHSILAT)", RegexOptions.IgnoreCase) ||
                                      Regex.IsMatch(sql, @"SIGN\s*=\s*1\s+THEN.*(?:CIKIS|CASH_OUT|NAKIT_CIKIS|GIDEN|ODEME)", RegexOptions.IgnoreCase);

                if (hasReversedSign && !hasCorrectSign)
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-07",
                        Title = "Ters Kasa SIGN Yön Kullanımı (KSLINES)",
                        PenaltyPoints = 10,
                        IssueDescription = "Kasa hareketlerinde SIGN = 1 giriş, SIGN = 0 çıkış olarak kullanılmış. Logo'da doğrusu: SIGN = 0 (Borç/Giriş), SIGN = 1 (Alacak/Çıkış).",
                        V26RuleReference = "Logo ERP v8.0 Madde 8: Hem BNFLINE hem KSLINES'da SIGN = 0 → Giriş, SIGN = 1 → Çıkış.",
                        RecommendedFix = "Nakit giriş: `SUM(CASE WHEN [SIGN] = 0 THEN [AMOUNT] ELSE 0 END)`, Nakit çıkış: `SUM(CASE WHEN [SIGN] = 1 THEN [AMOUNT] ELSE 0 END)`."
                    });
                }
                else if (hasCorrectSign)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-07",
                        Title = "Doğru Kasa SIGN Yönü (KSLINES)",
                        Description = "Kasa nakit akışında SIGN = 0 (Giriş) ve SIGN = 1 (Çıkış) doğru uygulanmış."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-08: INVOICE TRCODE PROFORMA CHECK - Penalty: -10 pts (Logo ERP v8.0 Madde 7)
            // TRCODE 10/13/14 = Proforma → ciro/tutar hesabına dahil edilmemeli
            // -------------------------------------------------------------
            if (upperSql.Contains("INVOICE") && (upperSql.Contains("SUM(") || upperSql.Contains("NETTOTAL") || upperSql.Contains("GROSSTOTAL")))
            {
                bool includesProforma = Regex.IsMatch(sql, @"TRCODE\s+IN\s*\([^)]*\b14\b[^)]*\)", RegexOptions.IgnoreCase) ||
                                        Regex.IsMatch(sql, @"TRCODE\s+IN\s*\([^)]*\b10\b[^)]*\)", RegexOptions.IgnoreCase) ||
                                        Regex.IsMatch(sql, @"TRCODE\s+IN\s*\([^)]*\b13\b[^)]*\)", RegexOptions.IgnoreCase);
                bool hasGrpCodeFilter = Regex.IsMatch(sql, @"GRPCODE\s*=\s*[12]", RegexOptions.IgnoreCase);

                if (includesProforma && hasGrpCodeFilter)
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-08",
                        Title = "Proforma Fatura Ciro Hesabına Dahil (TRCODE 10/13/14)",
                        PenaltyPoints = 10,
                        IssueDescription = "Fatura ciro/tutar hesabında Proforma fatura kodları (TRCODE 10, 13 veya 14) dahil edilmiş. Proforma faturalar gerçek gelir/gider yaratmaz.",
                        V26RuleReference = "Logo ERP v8.0 Madde 7: Satış faturaları TRCODE IN (7,8,9), Alış TRCODE IN (1,4). Proforma TRCODE IN (10,13,14) ciro/maliyet hesabına dahil edilmez.",
                        RecommendedFix = "Satış cirosu için: `GRPCODE = 2 AND TRCODE IN (7, 8, 9)`. Proforma kodları (10, 13, 14) çıkarın."
                    });
                }
                else if (hasGrpCodeFilter && !includesProforma)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-08",
                        Title = "Doğru Fatura TRCODE Ayrımı",
                        Description = "Fatura sorgusunda GRPCODE ve TRCODE filtresi uygulanmış; Proforma kodlar dahil edilmemiş."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-09: LINEEXP ISNULL PROTECTION - Penalty: -5 pts (Logo ERP v8.0 Madde 8)
            // -------------------------------------------------------------
            if (upperSql.Contains("LINEEXP") && (upperSql.Contains("FAIZ") || upperSql.Contains("FAİZ")))
            {
                bool hasIsnullProtection = upperSql.Contains("ISNULL(LINEEXP") || upperSql.Contains("ISNULL( LINEEXP") ||
                                           Regex.IsMatch(sql, @"ISNULL\s*\(\s*\[?\w*\.?\[?LINEEXP", RegexOptions.IgnoreCase);
                if (hasIsnullProtection)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-09",
                        Title = "LINEEXP NULL Koruması (Faiz Ayrımı)",
                        Description = "Banka kredi faiz/ana para ayrımında ISNULL(LINEEXP, N'') koruması uygulanmış."
                    });
                }
                else
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-09",
                        Title = "Eksik LINEEXP NULL Koruması",
                        PenaltyPoints = 5,
                        IssueDescription = "Banka kredi faiz ayrımında LINEEXP alanı ISNULL koruması olmadan kullanılmış. NULL satırlar yanlışlıkla ana paraya dahil edilebilir.",
                        V26RuleReference = "Logo ERP v8.0 Madde 8: Faiz ayrımı `UPPER(ISNULL(LINEEXP, N'')) LIKE UPPER(N'%faiz%')` şeklinde ISNULL korumalı olmalıdır.",
                        RecommendedFix = "`UPPER(BFL.[LINEEXP]) LIKE ...` yerine `UPPER(ISNULL(BFL.[LINEEXP], N'')) LIKE ...` kullanın."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-10: DATE RANGE OPEN INTERVAL - Penalty: -5 pts (Logo ERP v8.0 Madde 3)
            // -------------------------------------------------------------
            if (upperSql.Contains("BETWEEN") && (upperSql.Contains("DATE_") || upperSql.Contains("PROCDATE")))
            {
                bool usesUnsafeBetween = Regex.IsMatch(sql, @"BETWEEN\s+'\d{4}-\d{2}-\d{2}'\s+AND\s+'\d{4}-\d{2}-(28|29|30|31)'", RegexOptions.IgnoreCase);
                if (usesUnsafeBetween)
                {
                    score -= 5;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-10",
                        Title = "Riskli Tarih Aralığı Filtresi (BETWEEN)",
                        PenaltyPoints = 5,
                        IssueDescription = "Tarih filtrelerinde ayın son günü BETWEEN ile kısıtlandığında saat/zaman bileşeni nedeniyle son gün verileri kaçabilir.",
                        V26RuleReference = "Logo ERP v8.0 Madde 3: Tarih filtreleme açık aralık kullanılır: `WHERE DATE_ >= '2026-01-01' AND DATE_ < '2027-01-01'`.",
                        RecommendedFix = "`BETWEEN '2026-01-01' AND '2026-01-31'` yerine `>= '2026-01-01' AND < '2026-02-01'` yazın."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-11: BNFLINE SIGN DIRECTION CHECK - Penalty: -10 pts (Logo ERP v8.0 Madde 8)
            // -------------------------------------------------------------
            if (upperSql.Contains("BNFLINE"))
            {
                bool hasReversedSign = Regex.IsMatch(sql, @"SIGN\s*=\s*1\s+THEN.*(?:GIRIS|CASH_IN|NAKIT_GIRIS|GELEN|TAHSILAT)", RegexOptions.IgnoreCase) ||
                                       Regex.IsMatch(sql, @"SIGN\s*=\s*0\s+THEN.*(?:CIKIS|CASH_OUT|NAKIT_CIKIS|GIDEN|ODEME)", RegexOptions.IgnoreCase);
                bool hasCorrectSign = Regex.IsMatch(sql, @"SIGN\s*=\s*0\s+THEN.*(?:GIRIS|CASH_IN|NAKIT_GIRIS|GELEN|TAHSILAT)", RegexOptions.IgnoreCase) ||
                                      Regex.IsMatch(sql, @"SIGN\s*=\s*1\s+THEN.*(?:CIKIS|CASH_OUT|NAKIT_CIKIS|GIDEN|ODEME)", RegexOptions.IgnoreCase);

                if (hasReversedSign && !hasCorrectSign)
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-11",
                        Title = "Ters Banka SIGN Yön Kullanımı (BNFLINE)",
                        PenaltyPoints = 10,
                        IssueDescription = "Banka hareketlerinde SIGN = 1 giriş, SIGN = 0 çıkış olarak kullanılmış. Logo'da doğrusu: SIGN = 0 (Borç/Giriş), SIGN = 1 (Alacak/Çıkış).",
                        V26RuleReference = "Logo ERP v8.0 Madde 8: Hem BNFLINE hem KSLINES'da SIGN = 0 → Giriş, SIGN = 1 → Çıkış.",
                        RecommendedFix = "Banka giriş: `SUM(CASE WHEN [SIGN] = 0 THEN [AMOUNT] ELSE 0 END)`, Banka çıkış: `SUM(CASE WHEN [SIGN] = 1 THEN [AMOUNT] ELSE 0 END)`."
                    });
                }
                else if (hasCorrectSign)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-11",
                        Title = "Doğru Banka SIGN Yönü (BNFLINE)",
                        Description = "Banka nakit akışında SIGN = 0 (Giriş) ve SIGN = 1 (Çıkış) doğru uygulanmış."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-12: CLFLINE TRCODE <> 41 CHECK - Penalty: -10 pts (Logo ERP v8.0 Madde 5)
            // -------------------------------------------------------------
            if (upperSql.Contains("CLFLINE") && (upperSql.Contains("SUM(") || upperSql.Contains("AMOUNT")))
            {
                bool hasOzelFisExclusion = Regex.IsMatch(sql, @"TRCODE\s*<>\s*41", RegexOptions.IgnoreCase) ||
                                           Regex.IsMatch(sql, @"TRCODE\s*!=\s*41", RegexOptions.IgnoreCase) ||
                                           Regex.IsMatch(sql, @"TRCODE\s+NOT\s+IN\s*\([^)]*41[^)]*\)", RegexOptions.IgnoreCase);
                bool usesView = upperSql.Contains("LV_") && upperSql.Contains("CLFLINE");

                if (usesView)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-12",
                        Title = "Cari Hareket View Kullanımı (Özel Fiş N/A)",
                        Description = "LV_XXX_YY_CLFLINE view'ı kullanıldığı için DEBIT/CREDIT hazır — TRCODE <> 41 filtresi gerekmez."
                    });
                }
                else if (hasOzelFisExclusion)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-12",
                        Title = "Özel Fiş Hariç Tutulmuş (TRCODE <> 41)",
                        Description = "Cari hareket bakiye hesabında Özel Fiş (TRCODE = 41) doğru şekilde hariç tutulmuş."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-12",
                        Title = "Özel Fiş Dahil Net Bakiye Riski (TRCODE <> 41)",
                        PenaltyPoints = 10,
                        IssueDescription = "CLFLINE tablosunda bakiye/tutar hesaplanırken TRCODE <> 41 filtresi uygulanmamış. Özel Fiş (TRCODE = 41) bakiye kümülasyonunu bozabilir.",
                        V26RuleReference = "Logo ERP v8.0 Madde 5: Net bakiye hesabında TRCODE = 41 (Özel Fiş) hariç tutulur.",
                        RecommendedFix = "WHERE koşuluna `AND TRCODE <> 41` ekleyin veya LV_XXX_YY_CLFLINE view'ını kullanın."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-13: PAYTRANS OVERDUE FILTER SET - Penalty: -10 pts (Logo ERP v8.0 Madde 4)
            // -------------------------------------------------------------
            if (upperSql.Contains("PAYTRANS"))
            {
                bool hasClosedOrOpenBalance = Regex.IsMatch(sql, @"CLOSED\s*=\s*0", RegexOptions.IgnoreCase) ||
                                              Regex.IsMatch(sql, @"TOTAL\s*-\s*PAID", RegexOptions.IgnoreCase);
                bool hasCancelledFilter = Regex.IsMatch(sql, @"CANCELLED\s*=\s*0", RegexOptions.IgnoreCase);
                bool hasSignFilter = Regex.IsMatch(sql, @"SIGN\s*=\s*[01]", RegexOptions.IgnoreCase);
                bool hasProcDateFilter = upperSql.Contains("PROCDATE");

                int filterCount = (hasClosedOrOpenBalance ? 1 : 0) + (hasCancelledFilter ? 1 : 0) + (hasSignFilter ? 1 : 0) + (hasProcDateFilter ? 1 : 0);

                if (filterCount >= 3)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-13",
                        Title = "PAYTRANS Vadesi Geçmiş Filtre Seti",
                        Description = "Ödeme/tahsilat hareketlerinde CLOSED / (TOTAL-PAID), CANCELLED, SIGN ve PROCDATE filtrelerinin çoğunluğu uygulanmış."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-13",
                        Title = "Eksik PAYTRANS Filtre Seti",
                        PenaltyPoints = 10,
                        IssueDescription = $"PAYTRANS tablosunda açık fatura/alacak filtre setinden yalnızca {filterCount}/4 filtre uygulanmış. Tam set: (TOTAL - PAID) > 0 (veya CLOSED=0), CANCELLED=0, SIGN=0, PROCDATE < GETDATE().",
                        V26RuleReference = "Logo ERP v8.0 Madde 4: `WHERE (TOTAL - PAID) > 0 AND CANCELLED = 0 AND SIGN = 0 AND PROCDATE < CAST(GETDATE() AS date)`.",
                        RecommendedFix = "Eksik filtreleri ekleyin: (TOTAL - PAID) > 0 (veya CLOSED=0), CANCELLED=0 (aktif), SIGN=0 (alacak), PROCDATE < GETDATE() (vadesi geçmiş)."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-14: PRCLIST ACTIVE & PRICE TYPE CHECK - Penalty: -10 pts (Logo ERP v8.0 Madde 9 - YENİ)
            // -------------------------------------------------------------
            if (upperSql.Contains("PRCLIST"))
            {
                bool hasActiveFilter = Regex.IsMatch(sql, @"ACTIVE\s*=\s*0", RegexOptions.IgnoreCase);
                bool hasPTypeFilter = Regex.IsMatch(sql, @"PTYPE\s*=\s*[12]", RegexOptions.IgnoreCase) || Regex.IsMatch(sql, @"PTYPE\s+IN", RegexOptions.IgnoreCase);

                if (hasActiveFilter && hasPTypeFilter)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-14",
                        Title = "Fiyat Listesi Filtre Standardı (PRCLIST)",
                        Description = "LG_XXX_PRCLIST tablosunda ACTIVE = 0 ve PTYPE (1:Alış / 2:Satış) filtreleri doğru uygulanmış."
                    });
                }
                else if (!hasActiveFilter)
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-14",
                        Title = "PRCLIST Pasif Fiyat Filtresi Eksik",
                        PenaltyPoints = 10,
                        IssueDescription = "PRCLIST tablosunda ACTIVE = 0 filtresi eksik. Pasif veya iptal edilmiş fiyat listeleri sonuçlara karışabilir.",
                        V26RuleReference = "Logo ERP v8.0 Madde 9: Güncel satış fiyatı için `PRCLIST WHERE PTYPE=2, ACTIVE=0, BEGDATE <= GETDATE(), (ENDDATE >= GETDATE() OR ENDDATE IS NULL)` zorunludur.",
                        RecommendedFix = "WHERE koşuluna `AND ACTIVE = 0 AND PTYPE = 2` ekleyin."
                    });
                }
            }

            // -------------------------------------------------------------
            // RULE L-15: SLSCLREL SALES REP RELATION CHECK - Penalty: -10 pts (Logo ERP v8.0 Madde 9 - YENİ)
            // -------------------------------------------------------------
            if (upperSql.Contains("SLSCLREL"))
            {
                bool hasValidJoins = (upperSql.Contains("SLSMANREF") && upperSql.Contains("SLSMAN")) ||
                                     (upperSql.Contains("CLIENTREF") && upperSql.Contains("CLCARD"));
                if (hasValidJoins)
                {
                    report.PassedChecks.Add(new MikroRuleCheck
                    {
                        RuleId = "L-15",
                        Title = "Satış Temsilcisi Köprü İlişkisi (SLSCLREL)",
                        Description = "LG_SLSMAN -> LG_XXX_SLSCLREL -> LG_XXX_CLCARD köprü eşleştirmesi doğru kurulmuş."
                    });
                }
                else
                {
                    score -= 10;
                    report.Violations.Add(new MikroRuleViolation
                    {
                        RuleId = "L-15",
                        Title = "Hatalı Satış Temsilcisi Köprü Eşleşmesi",
                        PenaltyPoints = 10,
                        IssueDescription = "SLSCLREL tablosunda SLSMANREF (LG_SLSMAN) veya CLIENTREF (CLCARD) bağlantısı eksik.",
                        V26RuleReference = "Logo ERP v8.0 Madde 9: Satış temsilcisi eşleşmesi `LG_SLSMAN -> LG_XXX_SLSCLREL (SLSMANREF) -> LG_XXX_CLCARD (CLIENTREF)` şeklinde olmalıdır.",
                        RecommendedFix = "JOIN bağlantılarını SLSMANREF ve CLIENTREF üzerinden kurun."
                    });
                }
            }

            // Final score calculations
            score = Math.Max(0, Math.Min(100, score));
            report.Score = score;

            if (score >= 95)
            {
                report.Grade = "A+";
                report.GradeLabel = "Kusursuz Uyum (A+)";
                report.SummaryText = "T-SQL sorgusu Logo ERP v8.0 standartlarına ve şemasına %100 kusursuz uyum sağlamaktadır.";
            }
            else if (score >= 85)
            {
                report.Grade = "A";
                report.GradeLabel = "Yüksek Uyum (A)";
                report.SummaryText = "T-SQL sorgusu Logo ERP v8.0 kurallarına yüksek oranda uymaktadır.";
            }
            else if (score >= 70)
            {
                report.Grade = "B";
                report.GradeLabel = "Orta Uyum (B)";
                report.SummaryText = "Logo sorgusunda bazı v8.0 standart filtreleri (CANCELLED, ACTIVE, NOLOCK veya SIGN yönü) eksiktir.";
            }
            else if (score >= 50)
            {
                report.Grade = "C";
                report.GradeLabel = "Zayıf Uyum (C)";
                report.SummaryText = "Sorguda önemli Logo ERP v8.0 standart ihlalleri tespit edilmiştir.";
            }
            else
            {
                report.Grade = "F";
                report.GradeLabel = "Uyumsuz / Riskli (F)";
                report.SummaryText = "Sorgu Logo ERP v8.0 mimarisinden ciddi sapmalar göstermektedir.";
            }

            return report;
        }
    }
}
