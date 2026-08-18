using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OzBiPortalCRM.Data;

namespace OzBiPortalCRM.Services
{
    public class TenantSchemaProvider : ITenantSchemaProvider
    {
        private readonly IDbContextFactory<OzBiDbContext> _dbFactory;
        private readonly IMemoryCache _cache;

        public TenantSchemaProvider(IDbContextFactory<OzBiDbContext> dbFactory, IMemoryCache cache)
        {
            _dbFactory = dbFactory;
            _cache = cache;
        }

        public async Task<TenantErpConfig> GetTenantErpConfigAsync(string? tenantId, string? tenantName = null)
        {
            var config = new TenantErpConfig
            {
                TenantId = tenantId ?? string.Empty,
                TenantName = tenantName ?? string.Empty,
                ErpType = ErpSystemType.Generic
            };

            if (string.IsNullOrEmpty(tenantId))
            {
                if (!string.IsNullOrEmpty(tenantName))
                {
                    config.ErpType = DetectErpFromTenantName(tenantName);
                }
                return config;
            }

            var cacheKey = $"tenant_erp_cfg_{tenantId}";
            if (_cache.TryGetValue(cacheKey, out TenantErpConfig? cachedConfig) && cachedConfig != null)
            {
                return cachedConfig;
            }

            try
            {
                using var db = await _dbFactory.CreateDbContextAsync();

                // 1. Fetch Tenant
                var tenant = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
                if (tenant != null && string.IsNullOrEmpty(config.TenantName))
                {
                    config.TenantName = tenant.Name;
                }

                // 2. Fetch Active Data Connections for this tenant
                var connections = await db.Connections.AsNoTracking()
                    .Include(c => c.ConnectionSourceCode)
                    .Where(c => c.TenantId == tenantId && c.IsActive)
                    .ToListAsync();

                var erpConn = connections.FirstOrDefault(c =>
                    c.ConnectionSourceCode != null &&
                    ((c.ConnectionSourceCode.ProgrammaticName ?? "").ToUpperInvariant().Contains("MIKRO") ||
                     (c.ConnectionSourceCode.DisplayName_TR ?? "").ToUpperInvariant().Contains("MIKRO") ||
                     (c.ConnectionSourceCode.ProgrammaticName ?? "").ToUpperInvariant().Contains("LOGO") ||
                     (c.ConnectionSourceCode.DisplayName_TR ?? "").ToUpperInvariant().Contains("LOGO")))
                    ?? connections.FirstOrDefault();

                if (erpConn?.ConnectionSourceCode != null)
                {
                    config.ConnectionSourceName = erpConn.ConnectionSourceCode.DisplayName_TR ?? erpConn.ConnectionSourceCode.ProgrammaticName ?? string.Empty;
                    var progName = (erpConn.ConnectionSourceCode.ProgrammaticName ?? string.Empty).ToUpperInvariant();
                    var dispName = (erpConn.ConnectionSourceCode.DisplayName_TR ?? string.Empty).ToUpperInvariant();

                    if (progName.Contains("LOGO") || dispName.Contains("LOGO"))
                    {
                        config.ErpType = ErpSystemType.Logo;
                    }
                    else if (progName.Contains("MIKRO") || dispName.Contains("MIKRO"))
                    {
                        config.ErpType = ErpSystemType.Mikro;
                    }
                }

                // Fallback to tenant name heuristic if DB connection code is unassigned
                if (config.ErpType == ErpSystemType.Generic && !string.IsNullOrEmpty(config.TenantName))
                {
                    config.ErpType = DetectErpFromTenantName(config.TenantName);
                }

                // 3. Fetch Assistant Custom Prompt / Description if available
                var assistant = await db.Assistants.AsNoTracking()
                    .Where(a => a.TenantId == tenantId && a.IsActive)
                    .FirstOrDefaultAsync();

                if (assistant != null)
                {
                    config.CustomPromptRules = assistant.Description;
                }

                _cache.Set(cacheKey, config, TimeSpan.FromMinutes(15));
            }
            catch (Exception ex)
            {
                Console.WriteLine("TenantSchemaProvider error: " + ex.Message);
                if (!string.IsNullOrEmpty(config.TenantName))
                {
                    config.ErpType = DetectErpFromTenantName(config.TenantName);
                }
            }

            return config;
        }

        public ErpSystemType DetectErpTypeFromSql(string sqlQuery, string? tenantName = null)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery)) return ErpSystemType.Generic;
            var upperSql = sqlQuery.ToUpperInvariant();

            // Logo ERP signatures: LG_FFF_ or LG_001_ or CLCARD or INVOICE or STLINE
            if (upperSql.Contains("LG_") || upperSql.Contains("CLCARD") || upperSql.Contains("STLINE") || upperSql.Contains("LINETYPE"))
            {
                return ErpSystemType.Logo;
            }

            // Mikro ERP signatures: CARI_HESAPLAR, CARI_HESAP_HAREKETLERI, STOKLAR, STOK_HAREKETLERI
            if (upperSql.Contains("CARI_HESAP") || upperSql.Contains("STOKLAR") || upperSql.Contains("STOK_HAREKETLERI") || upperSql.Contains("SIPARISLER"))
            {
                return ErpSystemType.Mikro;
            }

            if (!string.IsNullOrEmpty(tenantName))
            {
                return DetectErpFromTenantName(tenantName);
            }

            return ErpSystemType.Generic;
        }

        private ErpSystemType DetectErpFromTenantName(string name)
        {
            var lowerName = name.ToLowerInvariant();
            if (lowerName.Contains("logo")) return ErpSystemType.Logo;
            if (lowerName.Contains("mikro")) return ErpSystemType.Mikro;
            return ErpSystemType.Generic;
        }
    }
}
