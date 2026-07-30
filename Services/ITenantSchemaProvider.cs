using System.Threading.Tasks;

namespace OzBiPortalCRM.Services
{
    public enum ErpSystemType
    {
        Mikro,
        Logo,
        Generic
    }

    public class TenantErpConfig
    {
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public ErpSystemType ErpType { get; set; } = ErpSystemType.Generic;
        public string ConnectionSourceName { get; set; } = string.Empty;
        public string? CustomPromptRules { get; set; }
        public string? CustomSchemaJson { get; set; }
    }

    public interface ITenantSchemaProvider
    {
        Task<TenantErpConfig> GetTenantErpConfigAsync(string? tenantId, string? tenantName = null);
        ErpSystemType DetectErpTypeFromSql(string sqlQuery, string? tenantName = null);
    }
}
