using System;
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
        public int TotalLogins { get; set; }
        public int QueryQuotaLimit { get; set; } = 50;
        public int RemainingQueries => Math.Max(0, QueryQuotaLimit - TotalQueries);
        public double QuotaUsagePercentage => QueryQuotaLimit > 0 ? Math.Min(100.0, Math.Round((double)TotalQueries / QueryQuotaLimit * 100, 1)) : 0;
        public DateTime? LastActivityDate { get; set; }
        public bool IsFavorited { get; set; }
    }

    public class UserAuditSummary
    {
        public OzBiUser User { get; set; } = null!;
        public string TenantName { get; set; } = string.Empty;
        public int TotalChats { get; set; }
        public int TotalMessages { get; set; }
        public int TotalQueries { get; set; }
        public int QueryQuotaLimit { get; set; } = 50;
        public int RemainingQueries => Math.Max(0, QueryQuotaLimit - TotalQueries);
        public double QuotaUsagePercentage => QueryQuotaLimit > 0 ? Math.Min(100.0, Math.Round((double)TotalQueries / QueryQuotaLimit * 100, 1)) : 0;
        public DateTime? LastActivityDate { get; set; }
        public bool IsFavorited { get; set; }
    }

    public class ChatAuditSummary
    {
        public OzBiChat Chat { get; set; } = null!;
        public int MessageCount { get; set; }
        public int QueryCount { get; set; }
        public long TotalDurationMs { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public string? PrimaryAiModelName { get; set; }
        public string? AssistantName { get; set; }
        public bool IsAsistantMode { get; set; }
        public List<string> UserQuestions { get; set; } = new();
    }

    public interface IOzBiAuditService
    {
        Task<List<TenantAuditSummary>> GetTenantsSummaryAsync(string? searchTerm = null, int portalUserId = 0);
        Task<List<UserAuditSummary>> GetUsersFootprintSummaryAsync(string? searchTerm = null, int portalUserId = 0);
        Task<OzBiTenant?> GetTenantByIdAsync(string tenantId);
        Task<List<ChatAuditSummary>> GetChatsForTenantAsync(string tenantId, string? searchTerm = null, string? filterUserId = null);
        Task<List<OzBiUser>> GetUsersForTenantAsync(string tenantId);
        Task<List<ChatAuditSummary>> GetChatsForUserAsync(string userId, string? searchTerm = null);
        Task<OzBiChat?> GetChatByIdAsync(string chatId);
        Task<List<OzBiChatMessage>> GetMessagesForChatAsync(string chatId);
        Task<List<OzBiChatMessage>> SearchGlobalQueriesAsync(
            string? tenantId = null,
            string? querySearch = null,
            bool? failedOnly = false,
            long? minDurationMs = null,
            int maxResults = 100);
        Task<Dictionary<string, int>> GetAiModelUsageStatsAsync();

        // Favorite & Footprint Persistence Methods
        Task<bool> ToggleFavoriteAsync(int portalUserId, string itemType, string itemId, string itemName, string? itemSubText);
        Task<List<FavoriteItem>> GetUserFavoritesAsync(int portalUserId);
        Task<HashSet<string>> GetFavoriteItemIdsAsync(int portalUserId, string itemType);

        // Tenant Subscription (SQLite & MariaDB) Methods
        Task<TenantSubscription?> GetTenantSubscriptionAsync(string tenantId);
        Task<bool> SaveTenantSubscriptionAsync(string tenantId, DateTime? subscriptionEndDate, string? sourceCampaign = null);
        Task<(DateTime? StartDate, DateTime? EndDate)> GetTenantSubscriptionFromMariaDbAsync(string tenantId, string? remoteId);

        // Tenant Compliance Scorecard (Multi-ERP & Prompt Sync) Methods
        Task<TenantComplianceScorecard> GetTenantComplianceScorecardAsync(string tenantId, bool forceRefresh = false);
        Task<Dictionary<string, TenantComplianceSnapshot>> GetAllTenantComplianceSnapshotsAsync();

        // Customer Feedbacks & Dislike Hub Methods
        Task<List<OzBiChatMessage>> GetCustomerFeedbacksAsync(
            string? tenantId = null,
            string? searchTerm = null,
            string? filterType = "disliked",
            int maxResults = 150);
        Task<FeedbackAuditStats> GetFeedbackStatsAsync(string? tenantId = null);
    }

    public class FeedbackAuditStats
    {
        public int TotalFeedbacks { get; set; }
        public int DislikedCount { get; set; }
        public int LikedCount { get; set; }
        public int WithCommentCount { get; set; }
        public int FailedAndDislikedCount { get; set; }
    }
}

