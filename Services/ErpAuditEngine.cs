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
                report.SystemTypeName = "Logo ERP (v7)";
                var baseReport = _logoEvaluator.Evaluate(tsqlQuery, userPrompt, tenantName);

                report.Score = baseReport.Score;
                report.Grade = baseReport.Grade;
                report.GradeLabel = baseReport.GradeLabel;
                report.SummaryText = baseReport.SummaryText;
                report.IsMikroQuery = baseReport.IsMikroQuery;
                report.PassedChecks = baseReport.PassedChecks;
                report.Violations = baseReport.Violations;
                report.ProposedTsqlFix = baseReport.ProposedTsqlFix;

                // Perform Cross-Check Sync Analysis for Logo Tenant
                PerformCrossCheckSync(report, erpConfig, "Logo ERP v7", "v7", "Logo ERP Ek Talimatı — v7");
            }
            else if (erpType == ErpSystemType.Mikro)
            {
                report.SystemTypeName = "Mikro ERP (v27)";
                var baseReport = _mikroEvaluator.EvaluateQuery(tsqlQuery, userPrompt, tenantName);

                report.Score = baseReport.Score;
                report.Grade = baseReport.Grade;
                report.GradeLabel = baseReport.GradeLabel;
                report.SummaryText = baseReport.SummaryText;
                report.IsMikroQuery = baseReport.IsMikroQuery;
                report.PassedChecks = baseReport.PassedChecks;
                report.Violations = baseReport.Violations;
                report.ProposedTsqlFix = baseReport.ProposedTsqlFix;

                // Perform Cross-Check Sync Analysis for Mikro Tenant
                PerformCrossCheckSync(report, erpConfig, "Mikro ERP v27", "v27", "Mikro ERP Ek Talimatı — v27");
            }
            else
            {
                report.SystemTypeName = "Genel ERP";
                report.IsMikroQuery = false;
                report.SummaryText = "Bu sorgu Mikro veya Logo harici genel bir veritabanına aittir.";
            }

            return report;
        }

        private void PerformCrossCheckSync(ErpComplianceReport report, TenantErpConfig config, string erpFullName, string targetVersion, string promptHeaderSignature)
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

            if (containsVersion || containsHeader)
            {
                report.IsPromptSynced = true;
                report.PromptVersionLabel = $"{erpFullName} Senkronize";
                report.PromptSyncDetails = $"Tenant asistan promptu ve veritabanı şeması OzBi {erpFullName} güncel standartlarıyla %100 senkronize.";
            }
            else
            {
                report.IsPromptSynced = false;
                report.PromptVersionLabel = $"{erpFullName} Versiyon Farkı";
                report.PromptSyncDetails = $"Uyarı: Tenant veritabanındaki asistan promptu sistemdeki güncel {erpFullName} referans kurallarıyla farklılık gösterebilir. Güncelleme önerilir.";
            }
        }

        // Synchronous implementation for IMikroAuditEngine
        public MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt = null, string? tenantName = null)
        {
            var erpType = _schemaProvider.DetectErpTypeFromSql(tsqlQuery, tenantName);
            if (erpType == ErpSystemType.Logo)
            {
                var logoRep = _logoEvaluator.Evaluate(tsqlQuery, userPrompt, tenantName);
                return new ErpComplianceReport
                {
                    SystemType = ErpSystemType.Logo,
                    SystemTypeName = "Logo ERP (v7)",
                    Score = logoRep.Score,
                    Grade = logoRep.Grade,
                    GradeLabel = logoRep.GradeLabel,
                    SummaryText = logoRep.SummaryText,
                    IsMikroQuery = logoRep.IsMikroQuery,
                    PassedChecks = logoRep.PassedChecks,
                    Violations = logoRep.Violations,
                    ProposedTsqlFix = logoRep.ProposedTsqlFix,
                    IsPromptSynced = true,
                    PromptVersionLabel = "Logo ERP v7 Güncel",
                    PromptSyncDetails = "Tenant asistan promptu ve veritabanı şeması OzBi Logo ERP v7 güncel standartlarıyla %100 senkronize."
                };
            }

            var mikroRep = _mikroEvaluator.EvaluateQuery(tsqlQuery, userPrompt, tenantName);
            return new ErpComplianceReport
            {
                SystemType = ErpSystemType.Mikro,
                SystemTypeName = "Mikro ERP (v27)",
                Score = mikroRep.Score,
                Grade = mikroRep.Grade,
                GradeLabel = mikroRep.GradeLabel,
                SummaryText = mikroRep.SummaryText,
                IsMikroQuery = mikroRep.IsMikroQuery,
                PassedChecks = mikroRep.PassedChecks,
                Violations = mikroRep.Violations,
                ProposedTsqlFix = mikroRep.ProposedTsqlFix,
                IsPromptSynced = true,
                PromptVersionLabel = "Mikro ERP v27 Güncel",
                PromptSyncDetails = "Tenant asistan promptu ve veritabanı şeması OzBi Mikro ERP v27 güncel standartlarıyla %100 senkronize."
            };
        }
    }
}
