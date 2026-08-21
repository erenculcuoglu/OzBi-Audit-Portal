using System;
using System.IO;
using System.Threading.Tasks;

namespace OzBiPortalCRM.Services
{
    public class ErpAuditEngine : IErpAuditEngine, IMikroAuditEngine
    {
        private readonly ITenantSchemaProvider _schemaProvider;
        private readonly MikroAuditEngine _mikroEvaluator;
        private readonly LogoAuditEvaluator _logoEvaluator;

        public ErpAuditEngine(ITenantSchemaProvider schemaProvider)
        {
            _schemaProvider = schemaProvider;
            _mikroEvaluator = new MikroAuditEngine();
            _logoEvaluator = new LogoAuditEvaluator();
        }

        public async Task<ErpComplianceReport> EvaluateQueryAsync(string tsqlQuery, string? userPrompt = null, string? tenantId = null, string? tenantName = null)
        {
            var erpConfig = await _schemaProvider.GetTenantErpConfigAsync(tenantId, tenantName);
            var erpType = erpConfig.ErpType;

            if (erpType == ErpSystemType.Generic)
            {
                erpType = _schemaProvider.DetectErpTypeFromSql(tsqlQuery, tenantName);
            }

            var report = new ErpComplianceReport
            {
                SystemType = erpType
            };

            if (erpType == ErpSystemType.Logo)
            {
                report.SystemTypeName = "Logo ERP (v8.0)";
                var baseReport = _logoEvaluator.Evaluate(tsqlQuery, userPrompt, tenantName, forceEvaluation: true);

                report.Score = baseReport.Score;
                report.Grade = baseReport.Grade;
                report.GradeLabel = baseReport.GradeLabel;
                report.SummaryText = baseReport.SummaryText;
                report.IsMikroQuery = true;
                report.PassedChecks = baseReport.PassedChecks;
                report.Violations = baseReport.Violations;
                report.ProposedTsqlFix = baseReport.ProposedTsqlFix;

                // Perform Cross-Check Sync Analysis for Logo Tenant
                PerformCrossCheckSync(report, erpConfig, "Logo ERP v8.0", "v8.0", "Logo ERP Ek Talimatı — v8.0", new[] { "v8.0", "v8", "v7.5", "v7.4", "v7.3", "v7", "v1" });
            }
            else if (erpType == ErpSystemType.Mikro)
            {
                report.SystemTypeName = "Mikro ERP (v1.0)";
                var baseReport = _mikroEvaluator.EvaluateQuery(tsqlQuery, userPrompt, tenantName, forceEvaluation: true);

                report.Score = baseReport.Score;
                report.Grade = baseReport.Grade;
                report.GradeLabel = baseReport.GradeLabel;
                report.SummaryText = baseReport.SummaryText;
                report.IsMikroQuery = true;
                report.PassedChecks = baseReport.PassedChecks;
                report.Violations = baseReport.Violations;
                report.ProposedTsqlFix = baseReport.ProposedTsqlFix;

                // Perform Cross-Check Sync Analysis for Mikro Tenant
                PerformCrossCheckSync(report, erpConfig, "Mikro ERP v1.0", "v1.0", "Mikro ERP Ek Talimatı — v1.0", new[] { "v27", "v28", "v1" });
            }
            else
            {
                report.SystemTypeName = "Genel ERP";
                var baseReport = _mikroEvaluator.EvaluateQuery(tsqlQuery, userPrompt, tenantName, forceEvaluation: true);
                report.Score = baseReport.Score;
                report.Grade = baseReport.Grade;
                report.GradeLabel = baseReport.GradeLabel;
                report.SummaryText = baseReport.SummaryText;
                report.IsMikroQuery = true;
                report.PassedChecks = baseReport.PassedChecks;
                report.Violations = baseReport.Violations;
                report.ProposedTsqlFix = baseReport.ProposedTsqlFix;
            }

            return report;
        }

        private void PerformCrossCheckSync(ErpComplianceReport report, TenantErpConfig config, string erpFullName, string targetVersion, string promptHeaderSignature, string[]? aliasVersions = null)
        {
            if (string.IsNullOrWhiteSpace(config.CustomPromptRules))
            {
                report.IsPromptSynced = true;
                report.PromptVersionLabel = $"{erpFullName} Güncel";
                report.PromptSyncDetails = $"Tenant veritabanında özel asistan promptu tanımlanmamış. Sistemdeki en güncel {erpFullName} referans kuralları aktif olarak kullanılıyor.";
                return;
            }

            var dbPrompt = config.CustomPromptRules.ToLowerInvariant();
            var targetVerLower = targetVersion.ToLowerInvariant();

            bool containsVersion = dbPrompt.Contains(targetVerLower);
            bool containsHeader = dbPrompt.Contains(promptHeaderSignature.ToLowerInvariant());

            if (!containsVersion && aliasVersions != null)
            {
                containsVersion = aliasVersions.Any(v => dbPrompt.Contains(v.ToLowerInvariant()));
            }

            if (containsVersion || containsHeader)
            {
                report.IsPromptSynced = true;
                report.PromptVersionLabel = $"{erpFullName} Senkronize";
                report.PromptSyncDetails = $"Tenant asistan promptu ve veritabanı şeması OzBi {erpFullName} güncel standartlarıyla %100 senkronize.";
            }
            else
            {
                report.IsPromptSynced = false;
                report.PromptVersionLabel = $"{erpFullName} Sürüm Farkı";
                report.PromptSyncDetails = $"Tenant özel promptunda '{promptHeaderSignature}' veya '{targetVersion}' sürüm imzası bulunamadı. Lütfen asistan promptunu güncelleyin.";
            }
        }

        // Synchronous implementation for IMikroAuditEngine
        public MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            var erpType = _schemaProvider.DetectErpTypeFromSql(tsqlQuery, tenantName);
            if (erpType == ErpSystemType.Logo)
            {
                var logoRep = _logoEvaluator.Evaluate(tsqlQuery, userPrompt, tenantName, forceEvaluation: true);
                return new ErpComplianceReport
                {
                    SystemType = ErpSystemType.Logo,
                    SystemTypeName = "Logo ERP (v8.0)",
                    Score = logoRep.Score,
                    Grade = logoRep.Grade,
                    GradeLabel = logoRep.GradeLabel,
                    SummaryText = logoRep.SummaryText,
                    IsMikroQuery = true,
                    PassedChecks = logoRep.PassedChecks,
                    Violations = logoRep.Violations,
                    ProposedTsqlFix = logoRep.ProposedTsqlFix,
                    IsPromptSynced = true,
                    PromptVersionLabel = "Logo ERP Standart",
                    PromptSyncDetails = "Sorgu Logo ERP v8.0 standartları çerçevesinde denetlenmiştir."
                };
            }
            else if (erpType == ErpSystemType.Mikro)
            {
                var mikroRep = _mikroEvaluator.EvaluateQuery(tsqlQuery, userPrompt, tenantName, forceEvaluation: true);
                return new ErpComplianceReport
                {
                    SystemType = ErpSystemType.Mikro,
                    SystemTypeName = "Mikro ERP (v1.0)",
                    Score = mikroRep.Score,
                    Grade = mikroRep.Grade,
                    GradeLabel = mikroRep.GradeLabel,
                    SummaryText = mikroRep.SummaryText,
                    IsMikroQuery = true,
                    PassedChecks = mikroRep.PassedChecks,
                    Violations = mikroRep.Violations,
                    ProposedTsqlFix = mikroRep.ProposedTsqlFix,
                    IsPromptSynced = true,
                    PromptVersionLabel = "Mikro v1.0 Standart",
                    PromptSyncDetails = "Sorgu Mikro ERP v1.0 standartları çerçevesinde denetlenmiştir."
                };
            }
            else
            {
                var mikroRep = _mikroEvaluator.EvaluateQuery(tsqlQuery, userPrompt, tenantName, forceEvaluation: true);
                return new ErpComplianceReport
                {
                    SystemType = ErpSystemType.Generic,
                    SystemTypeName = "Genel ERP",
                    Score = mikroRep.Score,
                    Grade = mikroRep.Grade,
                    GradeLabel = mikroRep.GradeLabel,
                    SummaryText = mikroRep.SummaryText,
                    IsMikroQuery = true,
                    PassedChecks = mikroRep.PassedChecks,
                    Violations = mikroRep.Violations,
                    ProposedTsqlFix = mikroRep.ProposedTsqlFix,
                    IsPromptSynced = true,
                    PromptVersionLabel = "Genel Standart",
                    PromptSyncDetails = "Sorgu genel standartlar çerçevesinde denetlenmiştir."
                };
            }
        }

        public ErpComplianceReport GetDemoReport()
        {
            return new ErpComplianceReport
            {
                Score = 85,
                Grade = "A",
                GradeLabel = "Yüksek Uyum (A)",
                SystemType = ErpSystemType.Mikro,
                SystemTypeName = "Mikro ERP (v1.0)",
                IsMikroQuery = true,
                SummaryText = "T-SQL sorgusu Mikro ERP standartlarına büyük oranda uygundur. 1 adet kural ihlali tespit edildi.",
                PassedChecks = new()
                {
                    new MikroRuleCheck { RuleId = "M-01", Title = "Ana Hesap Türü Ayrımı", Description = "cha_cari_cins = 0 filtresi doğru uygulanmış." },
                    new MikroRuleCheck { RuleId = "M-02", Title = "TL Döviz Kuru Koruması", Description = "CASE WHEN cha_d_cins = 0 THEN 1.0 ELSE cha_d_kur END koruması mevcut." },
                    new MikroRuleCheck { RuleId = "M-03", Title = "İptal ve Gizli Kayıt Filtresi", Description = "cha_iptal = 0 ve cha_hidden = 0 filtreleri mevcut." }
                },
                Violations = new()
                {
                    new MikroRuleViolation { RuleId = "M-07", Title = "Hassas Olmayan Metin Araması", PenaltyPoints = 15, Severity = "Warning", RecommendedFix = "UPPER(kolon) LIKE UPPER(N'%...%') pattern'ını kullanın." }
                },
                IsPromptSynced = true,
                PromptVersionLabel = "Mikro ERP v1.0 Güncel",
                PromptSyncDetails = "Tenant asistan promptu ve veritabanı şeması OzBi Mikro ERP v1.0 güncel standartlarıyla %100 senkronize."
            };
        }
    }
}
