using System.Threading.Tasks;

namespace OzBiPortalCRM.Services
{
    public class ErpComplianceReport : MikroComplianceReport
    {
        public ErpSystemType SystemType { get; set; } = ErpSystemType.Generic;
        public string SystemTypeName { get; set; } = "Genel ERP";
        
        // Cross-Check Sync Analysis Properties
        public bool IsPromptSynced { get; set; } = true;
        public string PromptVersionLabel { get; set; } = "Güncel";
        public string PromptSyncDetails { get; set; } = "Tenant asistan promptu ve veritabanı şeması sistem referans versiyonu ile senkronize durumda.";
    }

    public interface IErpAuditEngine
    {
        Task<ErpComplianceReport> EvaluateQueryAsync(string tsqlQuery, string? userPrompt = null, string? tenantId = null, string? tenantName = null);
    }
}
