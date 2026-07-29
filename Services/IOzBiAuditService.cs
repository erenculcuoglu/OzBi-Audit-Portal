using System.Collections.Generic;
using System.Threading.Tasks;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public class TenantAuditSummary
    {
        public OzBiTenant Tenant { get; set; } = null!;
        public int TotalChats { get; set; }
        public int TotalMessages { get; set; }
        public int TotalQueries { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }

    public class ChatAuditSummary
    {
        public OzBiChat Chat { get; set; } = null!;
        public int MessageCount { get; set; }
        public int QueryCount { get; set; }
        public long TotalDurationMs { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public string? PrimaryAiModelName { get; set; }
    }

    public interface IOzBiAuditService
    {
        Task<List<TenantAuditSummary>> GetTenantsSummaryAsync(string? searchTerm = null);
        Task<OzBiTenant?> GetTenantByIdAsync(string tenantId);
        Task<List<ChatAuditSummary>> GetChatsForTenantAsync(string tenantId, string? searchTerm = null);
        Task<OzBiChat?> GetChatByIdAsync(string chatId);
        Task<List<OzBiChatMessage>> GetMessagesForChatAsync(string chatId);
        Task<List<OzBiChatMessage>> SearchGlobalQueriesAsync(
            string? tenantId = null,
            string? querySearch = null,
            bool? failedOnly = false,
            long? minDurationMs = null,
            int maxResults = 100);
        Task<Dictionary<string, int>> GetAiModelUsageStatsAsync();
    }
}
