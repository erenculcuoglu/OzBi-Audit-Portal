using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Data;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public class OzBiAuditService : IOzBiAuditService
    {
        private readonly IDbContextFactory<OzBiDbContext> _dbFactory;
        private readonly IDbContextFactory<AppDbContext> _appDbFactory;
        private static bool _useDemoMode = false;

        public static bool UseDemoMode
        {
            get => _useDemoMode;
            set => _useDemoMode = value;
        }

        private readonly IServiceProvider _serviceProvider;
        private readonly IErpAuditEngine _erpAuditEngine;
        private readonly ITenantSchemaProvider _schemaProvider;

        public OzBiAuditService(
            IDbContextFactory<OzBiDbContext> dbFactory,
            IDbContextFactory<AppDbContext> appDbFactory,
            IServiceProvider serviceProvider,
            IErpAuditEngine erpAuditEngine,
            ITenantSchemaProvider schemaProvider)
        {
            _dbFactory = dbFactory;
            _appDbFactory = appDbFactory;
            _serviceProvider = serviceProvider;
            _erpAuditEngine = erpAuditEngine;
            _schemaProvider = schemaProvider;
        }

        public async Task<List<TenantAuditSummary>> GetTenantsSummaryAsync(string? searchTerm = null, int portalUserId = 0)
        {
            if (_useDemoMode)
            {
                return GetDemoTenantsSummary(searchTerm);
            }

            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();

                var query = db.Tenants.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(t => (t.Name != null && t.Name.ToLower().Contains(term)) ||
                                             (t.Email != null && t.Email.ToLower().Contains(term)) ||
                                             (t.Id != null && t.Id.ToLower().Contains(term)));
                }

                var tenants = await query.OrderByDescending(t => t.DateCreated).Take(200).ToListAsync();
                var tenantIds = tenants.Where(t => !string.IsNullOrEmpty(t.Id)).Select(t => t.Id).ToHashSet();

                var chats = await db.Chats.AsNoTracking()
                    .Where(c => !string.IsNullOrEmpty(c.TenantId) && tenantIds.Contains(c.TenantId))
                    .Select(c => new { c.Id, c.TenantId, c.DateCreated })
                    .ToListAsync();

                var chatIds = chats.Where(c => !string.IsNullOrEmpty(c.Id)).Select(c => c.Id).ToHashSet();

                var messages = await db.ChatMessages.AsNoTracking()
                    .Where(m => !string.IsNullOrEmpty(m.ChatId) && chatIds.Contains(m.ChatId))
                    .Select(m => new { m.ChatId, HasQuery = !string.IsNullOrEmpty(m.Query), m.DateCreated })
                    .ToListAsync();

                var tenantUsers = await db.Users.AsNoTracking()
                    .Where(u => !string.IsNullOrEmpty(u.TenantId) && tenantIds.Contains(u.TenantId))
                    .Select(u => new { u.TenantId, u.LoginCount })
                    .ToListAsync();

                var favoriteTenantIds = portalUserId > 0 ? await GetFavoriteItemIdsAsync(portalUserId, "Tenant") : new HashSet<string>();

                var result = new List<TenantAuditSummary>();

                foreach (var t in tenants)
                {
                    var tenantChats = chats.Where(c => c.TenantId == t.Id).ToList();
                    var tenantChatIds = tenantChats.Select(c => c.Id).ToHashSet();
                    var tenantMessages = messages.Where(m => tenantChatIds.Contains(m.ChatId)).ToList();
                    var tenantLogins = tenantUsers.Where(u => u.TenantId == t.Id).Sum(u => u.LoginCount);

                    var dateList = tenantChats.Where(c => c.DateCreated.HasValue).Select(c => c.DateCreated!.Value)
                        .Concat(tenantMessages.Where(m => m.DateCreated.HasValue).Select(m => m.DateCreated!.Value))
                        .ToList();

                    if (t.DateCreated.HasValue)
                    {
                        dateList.Add(t.DateCreated.Value);
                    }

                    DateTime? lastAct = dateList.Count > 0 ? dateList.Max() : t.DateCreated;

                    result.Add(new TenantAuditSummary
                    {
                        Tenant = t,
                        TotalChats = tenantChats.Count,
                        TotalMessages = tenantMessages.Count,
                        TotalQueries = tenantMessages.Count(m => m.HasQuery),
                        TotalLogins = tenantLogins,
                        LastActivityDate = lastAct,
                        IsFavorited = favoriteTenantIds.Contains(t.Id)
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"MySQL Veritabanı Sorgu Hatası: {ex.Message}", ex);
            }
        }

        public async Task<List<UserAuditSummary>> GetUsersFootprintSummaryAsync(string? searchTerm = null, int portalUserId = 0)
        {
            if (_useDemoMode)
            {
                return GetDemoUsersSummary(searchTerm);
            }

            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();

                var query = db.Users.AsNoTracking().Where(u => !u.IsDeleted).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    var matchingTenantIds = await db.Tenants.AsNoTracking()
                        .Where(t => t.Name != null && t.Name.ToLower().Contains(term))
                        .Select(t => t.Id)
                        .ToListAsync();

                    query = query.Where(u => (u.NameSurname != null && u.NameSurname.ToLower().Contains(term)) ||
                                             (u.Email != null && u.Email.ToLower().Contains(term)) ||
                                             (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                                             (matchingTenantIds.Contains(u.TenantId)));
                }

                var users = await query.Take(200).ToListAsync();
                var userIds = users.Select(u => u.Id).ToHashSet();
                var tenantIds = users.Select(u => u.TenantId).Where(tid => !string.IsNullOrEmpty(tid)).ToHashSet();

                var tenantMap = await db.Tenants.AsNoTracking()
                    .Where(t => tenantIds.Contains(t.Id))
                    .ToDictionaryAsync(t => t.Id, t => t.Name);

                var chats = await db.Chats.AsNoTracking()
                    .Where(c => !string.IsNullOrEmpty(c.CreatedByUserId) && userIds.Contains(c.CreatedByUserId))
                    .Select(c => new { c.Id, c.CreatedByUserId, c.DateCreated })
                    .ToListAsync();

                var chatIds = chats.Select(c => c.Id).ToHashSet();

                var messages = await db.ChatMessages.AsNoTracking()
                    .Where(m => !string.IsNullOrEmpty(m.ChatId) && chatIds.Contains(m.ChatId))
                    .Select(m => new { m.ChatId, HasQuery = !string.IsNullOrEmpty(m.Query) })
                    .ToListAsync();

                var favoriteUserIds = portalUserId > 0 ? await GetFavoriteItemIdsAsync(portalUserId, "User") : new HashSet<string>();

                var result = new List<UserAuditSummary>();

                foreach (var u in users)
                {
                    var userChats = chats.Where(c => c.CreatedByUserId == u.Id).ToList();
                    var userChatIds = userChats.Select(c => c.Id).ToHashSet();
                    var userMessages = messages.Where(m => userChatIds.Contains(m.ChatId)).ToList();

                    tenantMap.TryGetValue(u.TenantId, out var tenantName);

                    DateTime? lastAct = null;
                    if (userChats.Count > 0)
                    {
                        var dates = userChats.Where(c => c.DateCreated.HasValue).Select(c => c.DateCreated!.Value).ToList();
                        if (dates.Count > 0) lastAct = dates.Max();
                    }

                    result.Add(new UserAuditSummary
                    {
                        User = u,
                        TenantName = tenantName ?? u.TenantId,
                        TotalChats = userChats.Count,
                        TotalMessages = userMessages.Count,
                        TotalQueries = userMessages.Count(m => m.HasQuery),
                        LastActivityDate = lastAct,
                        IsFavorited = favoriteUserIds.Contains(u.Id)
                    });
                }

                return result.OrderByDescending(r => r.IsFavorited).ThenByDescending(r => r.TotalChats).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Kullanıcı Ayak İzi Sorgu Hatası: {ex.Message}", ex);
            }
        }

        public async Task<OzBiTenant?> GetTenantByIdAsync(string tenantId)
        {
            if (_useDemoMode) return GetDemoTenantsSummary().FirstOrDefault(t => t.Tenant.Id == tenantId)?.Tenant;

            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<List<ChatAuditSummary>> GetChatsForTenantAsync(string tenantId, string? searchTerm = null, string? filterUserId = null)
        {
            if (_useDemoMode) return GetDemoChatsForTenant(tenantId, searchTerm);

            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = db.Chats.AsNoTracking()
                .Include(c => c.CreatedByUser)
                .Where(c => c.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(filterUserId))
            {
                query = query.Where(c => c.CreatedByUserId == filterUserId);
            }

            var chats = await query.OrderByDescending(c => c.DateCreated).ToListAsync();
            var chatIds = chats.Select(c => c.Id).ToList();

            var messages = await db.ChatMessages.AsNoTracking()
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                .Where(m => chatIds.Contains(m.ChatId))
                .OrderBy(m => m.DateCreated)
                .ToListAsync();

            var result = new List<ChatAuditSummary>();
            var term = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim().ToLower();

            foreach (var c in chats)
            {
                var msgList = messages.Where(m => m.ChatId == c.Id).ToList();
                var primaryModel = msgList.FirstOrDefault(m => m.AIModel != null)?.AIModel?.Name ?? "Standart AI";
                var assistantName = msgList.FirstOrDefault(m => m.Assistant != null)?.Assistant?.Name;

                // Turn-based question counting (same logic as ChatDetail.razor GetConversationTurns)
                var userQuestions = new List<string>();
                foreach (var m in msgList.OrderBy(m => m.DateCreated))
                {
                    bool isUser = m.Role?.ToLower() == "user";
                    var promptText = !string.IsNullOrWhiteSpace(m.Prompt)
                        ? m.Prompt
                        : (isUser ? m.Message : null);
                    if (!string.IsNullOrWhiteSpace(promptText) && !userQuestions.Contains(promptText.Trim()))
                    {
                        userQuestions.Add(promptText.Trim());
                    }
                }

                if (term != null)
                {
                    bool titleMatches = c.Title != null && c.Title.ToLower().Contains(term);
                    bool questionMatches = userQuestions.Any(q => q.ToLower().Contains(term));
                    bool messageMatches = msgList.Any(m => (m.Message != null && m.Message.ToLower().Contains(term)) || (m.Prompt != null && m.Prompt.ToLower().Contains(term)));

                    if (!titleMatches && !questionMatches && !messageMatches)
                    {
                        continue;
                    }
                }

                var userQuestionCount = userQuestions.Count;
                if (userQuestionCount == 0 && msgList.Count > 0) userQuestionCount = 1;

                var firstTurnMsg = msgList.OrderBy(m => m.DateCreated).FirstOrDefault(m => m.Role?.ToLower() != "user");
                bool isAssistantMode = firstTurnMsg != null ? firstTurnMsg.IsAssistantModeEffective : msgList.Any(m => m.IsAssistantModeEffective);

                result.Add(new ChatAuditSummary
                {
                    Chat = c,
                    MessageCount = userQuestionCount,
                    QueryCount = CountSqlItems(msgList),
                    TotalDurationMs = msgList.Sum(m => m.TotalDurationMs ?? 0),
                    LastMessageDate = msgList.Count > 0 ? msgList.Max(m => m.DateCreated) : c.DateCreated,
                    PrimaryAiModelName = primaryModel,
                    AssistantName = assistantName,
                    IsAsistantMode = isAssistantMode,
                    UserQuestions = userQuestions
                });
            }

            return result;
        }

        public async Task<List<OzBiUser>> GetUsersForTenantAsync(string tenantId)
        {
            if (_useDemoMode) return new List<OzBiUser>();

            await using var db = await _dbFactory.CreateDbContextAsync();

            var tenantUserIds = await db.Chats.AsNoTracking()
                .Where(c => c.TenantId == tenantId && !string.IsNullOrEmpty(c.CreatedByUserId))
                .Select(c => c.CreatedByUserId!)
                .Distinct()
                .ToListAsync();

            return await db.Users.AsNoTracking()
                .Where(u => u.TenantId == tenantId || tenantUserIds.Contains(u.Id))
                .OrderBy(u => u.NameSurname ?? u.Email ?? u.UserName)
                .ToListAsync();
        }

        public async Task<List<ChatAuditSummary>> GetChatsForUserAsync(string userId, string? searchTerm = null)
        {
            if (_useDemoMode) return GetDemoChatsForTenant("demo-tenant-1", searchTerm);

            await using var db = await _dbFactory.CreateDbContextAsync();

            var chats = await db.Chats.AsNoTracking()
                .Where(c => c.CreatedByUserId == userId)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();

            var chatIds = chats.Select(c => c.Id).ToList();

            var messages = await db.ChatMessages.AsNoTracking()
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                .Where(m => chatIds.Contains(m.ChatId))
                .OrderBy(m => m.DateCreated)
                .ToListAsync();

            var result = new List<ChatAuditSummary>();
            var term = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim().ToLower();

            foreach (var c in chats)
            {
                var msgList = messages.Where(m => m.ChatId == c.Id).ToList();
                var primaryModel = msgList.FirstOrDefault(m => m.AIModel != null)?.AIModel?.Name ?? "Standart AI";
                var assistantName = msgList.FirstOrDefault(m => m.Assistant != null)?.Assistant?.Name;

                // Turn-based question counting (same logic as ChatDetail.razor GetConversationTurns)
                var userQuestions = new List<string>();
                foreach (var m in msgList.OrderBy(m => m.DateCreated))
                {
                    bool isUser = m.Role?.ToLower() == "user";
                    var promptText = !string.IsNullOrWhiteSpace(m.Prompt)
                        ? m.Prompt
                        : (isUser ? m.Message : null);
                    if (!string.IsNullOrWhiteSpace(promptText) && !userQuestions.Contains(promptText.Trim()))
                    {
                        userQuestions.Add(promptText.Trim());
                    }
                }

                if (term != null)
                {
                    bool titleMatches = c.Title != null && c.Title.ToLower().Contains(term);
                    bool questionMatches = userQuestions.Any(q => q.ToLower().Contains(term));
                    bool messageMatches = msgList.Any(m => (m.Message != null && m.Message.ToLower().Contains(term)) || (m.Prompt != null && m.Prompt.ToLower().Contains(term)));

                    if (!titleMatches && !questionMatches && !messageMatches)
                    {
                        continue;
                    }
                }

                var userQuestionCount = userQuestions.Count;
                if (userQuestionCount == 0 && msgList.Count > 0) userQuestionCount = 1;

                var firstTurnMsg = msgList.OrderBy(m => m.DateCreated).FirstOrDefault(m => m.Role?.ToLower() != "user");
                bool isAssistantMode = firstTurnMsg != null ? firstTurnMsg.IsAssistantModeEffective : msgList.Any(m => m.IsAssistantModeEffective);

                result.Add(new ChatAuditSummary
                {
                    Chat = c,
                    MessageCount = userQuestionCount,
                    QueryCount = CountSqlItems(msgList),
                    TotalDurationMs = msgList.Sum(m => m.TotalDurationMs ?? 0),
                    LastMessageDate = msgList.Count > 0 ? msgList.Max(m => m.DateCreated) : c.DateCreated,
                    PrimaryAiModelName = primaryModel,
                    AssistantName = assistantName,
                    IsAsistantMode = isAssistantMode,
                    UserQuestions = userQuestions
                });
            }

            return result;
        }

        /// <summary>
        /// Counts actual SQL items from message Query fields by parsing JSON arrays/objects.
        /// Mirrors the ExtractSqlItemsFromQuery logic in ChatDetail.razor for consistent counts.
        /// </summary>
        private int CountSqlItems(List<OzBiChatMessage> messages)
        {
            int count = 0;
            foreach (var m in messages)
            {
                if (string.IsNullOrWhiteSpace(m.Query)) continue;
                var trimmed = m.Query.Trim();

                if ((trimmed.StartsWith("[") && trimmed.EndsWith("]")) || (trimmed.StartsWith("{") && trimmed.EndsWith("}")))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(trimmed);
                        var root = doc.RootElement;
                        if (root.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var elem in root.EnumerateArray())
                            {
                                if (HasSqlContent(elem)) count++;
                            }
                        }
                        else if (root.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            if (HasSqlContent(root)) count++;
                        }
                        continue;
                    }
                    catch
                    {
                        // Not valid JSON, fall through to raw count
                    }
                }

                // Raw SQL string
                count++;
            }
            return count;
        }

        private bool HasSqlContent(System.Text.Json.JsonElement elem)
        {
            string[] sqlKeys = { "sql", "Sql", "query", "Query", "tSql" };
            if (elem.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                foreach (var key in sqlKeys)
                {
                    if (elem.TryGetProperty(key, out var prop) &&
                        prop.ValueKind == System.Text.Json.JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(prop.GetString()))
                    {
                        return true;
                    }
                }
            }
            // A plain string element in an array
            if (elem.ValueKind == System.Text.Json.JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(elem.GetString()))
            {
                return true;
            }
            return false;
        }

        public async Task<OzBiChat?> GetChatByIdAsync(string chatId)
        {
            if (_useDemoMode) return new OzBiChat { Id = chatId, Title = "Demo Analiz Sohbeti", TenantId = "demo-tenant-1", DateCreated = DateTime.Now.AddDays(-1) };

            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Chats.AsNoTracking()
                .Include(c => c.Tenant)
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<List<OzBiChatMessage>> GetMessagesForChatAsync(string chatId)
        {
            if (_useDemoMode) return GetDemoMessagesForChat(chatId);

            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.ChatMessages.AsNoTracking()
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                    .ThenInclude(a => a!.DataConnection)
                        .ThenInclude(c => c!.ConnectionSourceCode)
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.DateCreated)
                .ToListAsync();
        }

        public async Task<List<OzBiChatMessage>> SearchGlobalQueriesAsync(
            string? tenantId = null,
            string? querySearch = null,
            bool? failedOnly = false,
            long? minDurationMs = null,
            int maxResults = 100)
        {
            if (_useDemoMode) return GetDemoGlobalQueries(querySearch, failedOnly, minDurationMs);

            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = db.ChatMessages.AsNoTracking()
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.Tenant)
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.CreatedByUser)
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                .Where(m => !string.IsNullOrEmpty(m.Query));

            if (!string.IsNullOrWhiteSpace(tenantId))
                query = query.Where(m => m.Chat != null && m.Chat.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(querySearch))
            {
                var term = querySearch.Trim().ToLower();
                query = query.Where(m => m.Query!.ToLower().Contains(term) || (m.Prompt != null && m.Prompt.ToLower().Contains(term)));
            }

            if (failedOnly == true)
                query = query.Where(m => !string.IsNullOrEmpty(m.ErrorMessage));

            if (minDurationMs.HasValue && minDurationMs.Value > 0)
                query = query.Where(m => m.TotalDurationMs >= minDurationMs.Value);

            var messages = await query.OrderByDescending(m => m.DateCreated).Take(maxResults).ToListAsync();
            var chatIds = messages.Select(m => m.ChatId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();

            if (chatIds.Count > 0)
            {
                var userMessages = await db.ChatMessages.AsNoTracking()
                    .Where(m => chatIds.Contains(m.ChatId) && (m.Role == "user" || m.Role == "User"))
                    .Select(m => new { m.ChatId, m.Message, m.Prompt, m.DateCreated })
                    .ToListAsync();

                foreach (var msg in messages)
                {
                    if (string.IsNullOrWhiteSpace(msg.Prompt))
                    {
                        var userPrompt = userMessages
                            .Where(u => u.ChatId == msg.ChatId && u.DateCreated <= msg.DateCreated)
                            .OrderByDescending(u => u.DateCreated)
                            .FirstOrDefault();

                        if (userPrompt != null)
                        {
                            msg.Prompt = !string.IsNullOrWhiteSpace(userPrompt.Message) ? userPrompt.Message : userPrompt.Prompt;
                        }
                    }
                }
            }

            return messages;
        }

        public async Task<Dictionary<string, int>> GetAiModelUsageStatsAsync()
        {
            if (_useDemoMode) return new Dictionary<string, int> { { "GPT-4o", 145 }, { "Claude 3.5 Sonnet", 98 }, { "DeepSeek V3", 42 } };

            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.ChatMessages.AsNoTracking()
                .Where(m => m.AIModel != null && m.AIModel.Name != null)
                .GroupBy(m => m.AIModel!.Name!)
                .Select(g => new { ModelName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ModelName, x => x.Count);
        }

        #region Favorite & Footprint Persistence Methods
        public async Task<bool> ToggleFavoriteAsync(int portalUserId, string itemType, string itemId, string itemName, string? itemSubText)
        {
            await using var db = await _appDbFactory.CreateDbContextAsync();
            await db.EnsureTablesCreatedAsync();

            var existing = await db.Favorites.FirstOrDefaultAsync(f => f.PortalUserId == portalUserId && f.ItemType == itemType && f.ItemId == itemId);

            if (existing != null)
            {
                db.Favorites.Remove(existing);
                await db.SaveChangesAsync();
                return false; // Removed from favorites
            }
            else
            {
                var newFav = new FavoriteItem
                {
                    PortalUserId = portalUserId,
                    ItemType = itemType,
                    ItemId = itemId,
                    ItemName = itemName,
                    ItemSubText = itemSubText,
                    AddedAt = DateTime.UtcNow
                };
                db.Favorites.Add(newFav);
                await db.SaveChangesAsync();
                return true; // Added to favorites
            }
        }

        public async Task<List<FavoriteItem>> GetUserFavoritesAsync(int portalUserId)
        {
            await using var db = await _appDbFactory.CreateDbContextAsync();
            await db.EnsureTablesCreatedAsync();
            return await db.Favorites.AsNoTracking()
                .Where(f => f.PortalUserId == portalUserId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();
        }

        public async Task<HashSet<string>> GetFavoriteItemIdsAsync(int portalUserId, string itemType)
        {
            await using var db = await _appDbFactory.CreateDbContextAsync();
            await db.EnsureTablesCreatedAsync();
            var ids = await db.Favorites.AsNoTracking()
                .Where(f => f.PortalUserId == portalUserId && f.ItemType == itemType)
                .Select(f => f.ItemId)
                .ToListAsync();
            return ids.ToHashSet();
        }

        public async Task<TenantSubscription?> GetTenantSubscriptionAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) return null;

            try
            {
                await using var db = await _appDbFactory.CreateDbContextAsync();
                await db.EnsureTablesCreatedAsync();

                return await db.TenantSubscriptions.AsNoTracking()
                    .FirstOrDefaultAsync(ts => ts.TenantId == tenantId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tenant subscription from SQLite: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveTenantSubscriptionAsync(string tenantId, DateTime? subscriptionEndDate, string? sourceCampaign = null)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) return false;

            try
            {
                await using var db = await _appDbFactory.CreateDbContextAsync();
                await db.EnsureTablesCreatedAsync();

                var sub = await db.TenantSubscriptions.FirstOrDefaultAsync(ts => ts.TenantId == tenantId);
                if (sub == null)
                {
                    sub = new TenantSubscription
                    {
                        TenantId = tenantId,
                        SubscriptionEndDate = subscriptionEndDate,
                        SourceCampaign = sourceCampaign,
                        LastUpdatedAt = DateTime.UtcNow
                    };
                    db.TenantSubscriptions.Add(sub);
                }
                else
                {
                    sub.SubscriptionEndDate = subscriptionEndDate;
                    if (sourceCampaign != null)
                    {
                        sub.SourceCampaign = sourceCampaign;
                    }
                    sub.LastUpdatedAt = DateTime.UtcNow;
                    db.TenantSubscriptions.Update(sub);
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tenant subscription to SQLite: {ex.Message}");
                return false;
            }
        }

        public async Task<(DateTime? StartDate, DateTime? EndDate)> GetTenantSubscriptionFromMariaDbAsync(string tenantId, string? remoteId)
        {
            if (string.IsNullOrWhiteSpace(tenantId) && string.IsNullOrWhiteSpace(remoteId))
                return (null, null);

            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();
                var conn = db.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT StartDate, EndDate 
                    FROM ozbiappc_portal.subscription 
                    WHERE TenantId = @id1 OR TenantId = @id2 
                    ORDER BY DateCreated DESC 
                    LIMIT 1;";

                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@id1";
                p1.Value = tenantId ?? (object)DBNull.Value;
                cmd.Parameters.Add(p1);

                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@id2";
                p2.Value = remoteId ?? (object)DBNull.Value;
                cmd.Parameters.Add(p2);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    DateTime? startDate = reader.IsDBNull(0) ? null : reader.GetDateTime(0);
                    DateTime? endDate = reader.IsDBNull(1) ? null : reader.GetDateTime(1);
                    return (startDate, endDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying ozbiappc_portal.subscription from MariaDB: {ex.Message}");
            }

            return (null, null);
        }
        #endregion

        #region Tenant Compliance Scorecard Methods
        public async Task<TenantComplianceScorecard> GetTenantComplianceScorecardAsync(string tenantId, bool forceRefresh = false)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return new TenantComplianceScorecard();

            if (_useDemoMode)
                return GetDemoComplianceScorecard(tenantId);

            try
            {
                await using var appDb = await _appDbFactory.CreateDbContextAsync();
                await appDb.EnsureTablesCreatedAsync();

                // 1. Check cached snapshot if not forcing refresh
                if (!forceRefresh)
                {
                    var snapshot = await appDb.TenantComplianceSnapshots.AsNoTracking()
                        .FirstOrDefaultAsync(s => s.TenantId == tenantId);

                    if (snapshot != null && snapshot.LastEvaluatedAt > DateTime.UtcNow.AddHours(-12))
                    {
                        var scorecard = new TenantComplianceScorecard
                        {
                            TenantId = snapshot.TenantId,
                            TenantName = snapshot.TenantName,
                            ErpType = snapshot.ErpType,
                            ErpTypeName = snapshot.ErpTypeName,
                            OverallScore = snapshot.OverallScore,
                            Grade = snapshot.Grade,
                            GradeLabel = snapshot.GradeLabel,
                            TotalQueriesEvaluated = snapshot.TotalQueriesEvaluated,
                            CompliantCount = snapshot.CompliantCount,
                            WarningCount = snapshot.WarningCount,
                            CriticalCount = snapshot.CriticalCount,
                            IsPromptSynced = snapshot.IsPromptSynced,
                            PromptVersionLabel = snapshot.PromptVersionLabel,
                            PromptSyncDetails = snapshot.PromptSyncDetails ?? string.Empty,
                            LastEvaluatedAt = snapshot.LastEvaluatedAt
                        };

                        if (!string.IsNullOrEmpty(snapshot.TopViolationsJson))
                        {
                            try
                            {
                                scorecard.TopViolations = System.Text.Json.JsonSerializer.Deserialize<List<TenantRuleViolationStat>>(snapshot.TopViolationsJson) ?? new();
                            }
                            catch { }
                        }

                        return scorecard;
                    }
                }

                // 2. Fetch Tenant details and latest SQL query messages from MariaDB
                await using var db = await _dbFactory.CreateDbContextAsync();
                var tenant = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
                var tenantName = tenant?.Name ?? "Tenant";

                var erpConfig = await _schemaProvider.GetTenantErpConfigAsync(tenantId, tenantName);

                var queryMessages = await db.ChatMessages.AsNoTracking()
                    .Include(m => m.Chat)
                    .Where(m => m.Chat != null && m.Chat.TenantId == tenantId && !string.IsNullOrEmpty(m.Query))
                    .OrderByDescending(m => m.DateCreated)
                    .Take(75)
                    .ToListAsync();

                var evaluatedList = new List<TenantQueryComplianceSummary>();
                var allViolations = new List<MikroRuleViolation>();
                var scores = new List<int>();

                ErpComplianceReport? lastReport = null;

                foreach (var msg in queryMessages)
                {
                    if (string.IsNullOrWhiteSpace(msg.Query)) continue;

                    var report = await _erpAuditEngine.EvaluateQueryAsync(msg.Query, msg.Prompt, tenantId, tenantName);
                    lastReport = report;

                    scores.Add(report.Score);
                    allViolations.AddRange(report.Violations);

                    evaluatedList.Add(new TenantQueryComplianceSummary
                    {
                        MessageId = msg.Id,
                        ChatId = msg.ChatId,
                        QuestionText = msg.Prompt ?? msg.Message,
                        SqlQuery = msg.Query,
                        Score = report.Score,
                        Grade = report.Grade,
                        GradeLabel = report.GradeLabel,
                        IsSucceeded = msg.IsSucceeded,
                        ErrorMessage = msg.ErrorMessage,
                        DateCreated = msg.DateCreated,
                        ViolationTitles = report.Violations.Select(v => v.Title).ToList()
                    });
                }

                int totalCount = scores.Count;
                int overallScore = totalCount > 0 ? (int)Math.Round(scores.Average()) : 100;

                string grade;
                string gradeLabel;
                if (overallScore >= 95) { grade = "A+"; gradeLabel = "Mükemmel"; }
                else if (overallScore >= 85) { grade = "A"; gradeLabel = "Çok İyi"; }
                else if (overallScore >= 70) { grade = "B"; gradeLabel = "İyi / Standart"; }
                else if (overallScore >= 55) { grade = "C"; gradeLabel = "Geliştirilmeli"; }
                else { grade = "F"; gradeLabel = "Kritik Uyumsuz"; }

                int compliant = scores.Count(s => s >= 90);
                int warning = scores.Count(s => s >= 60 && s < 90);
                int critical = scores.Count(s => s < 60);

                // Group top violations by RuleId/Title
                var topViolations = allViolations
                    .GroupBy(v => v.RuleId)
                    .Select(g =>
                    {
                        var first = g.First();
                        var count = g.Count();
                        var pct = totalCount > 0 ? Math.Round((double)count / totalCount * 100, 1) : 0;
                        return new TenantRuleViolationStat
                        {
                            RuleId = g.Key,
                            Title = first.Title,
                            Severity = first.Severity,
                            Count = count,
                            Percentage = pct,
                            TotalPenaltyPoints = g.Sum(x => x.PenaltyPoints),
                            RecommendedFix = first.RecommendedFix,
                            V26RuleReference = first.V26RuleReference
                        };
                    })
                    .OrderByDescending(v => v.Count)
                    .ThenByDescending(v => v.TotalPenaltyPoints)
                    .Take(6)
                    .ToList();

                var erpTypeName = erpConfig.ErpType == ErpSystemType.Logo ? "Logo ERP (v8.0)" :
                                  erpConfig.ErpType == ErpSystemType.Mikro ? "Mikro ERP (v1.0)" : "Genel ERP";

                bool isPromptSynced = lastReport?.IsPromptSynced ?? true;
                string promptVerLabel = lastReport?.PromptVersionLabel ?? "Güncel";
                string promptSyncDetails = lastReport?.PromptSyncDetails ?? "Tenant şeması ve kuralları güncel sistem standartlarıyla senkronize.";

                var resultScorecard = new TenantComplianceScorecard
                {
                    TenantId = tenantId,
                    TenantName = tenantName,
                    ErpType = erpConfig.ErpType.ToString(),
                    ErpTypeName = erpTypeName,
                    OverallScore = overallScore,
                    Grade = grade,
                    GradeLabel = gradeLabel,
                    TotalQueriesEvaluated = totalCount,
                    CompliantCount = compliant,
                    WarningCount = warning,
                    CriticalCount = critical,
                    IsPromptSynced = isPromptSynced,
                    PromptVersionLabel = promptVerLabel,
                    PromptSyncDetails = promptSyncDetails,
                    TopViolations = topViolations,
                    EvaluatedQueries = evaluatedList,
                    LastEvaluatedAt = DateTime.UtcNow
                };

                // Persist snapshot to SQLite
                var topViolationsJson = System.Text.Json.JsonSerializer.Serialize(topViolations);
                var existingSnapshot = await appDb.TenantComplianceSnapshots.FirstOrDefaultAsync(s => s.TenantId == tenantId);
                if (existingSnapshot == null)
                {
                    existingSnapshot = new TenantComplianceSnapshot
                    {
                        TenantId = tenantId,
                        TenantName = tenantName,
                        ErpType = erpConfig.ErpType.ToString(),
                        ErpTypeName = erpTypeName,
                        OverallScore = overallScore,
                        Grade = grade,
                        GradeLabel = gradeLabel,
                        TotalQueriesEvaluated = totalCount,
                        CompliantCount = compliant,
                        WarningCount = warning,
                        CriticalCount = critical,
                        IsPromptSynced = isPromptSynced,
                        PromptVersionLabel = promptVerLabel,
                        PromptSyncDetails = promptSyncDetails,
                        TopViolationsJson = topViolationsJson,
                        LastEvaluatedAt = DateTime.UtcNow
                    };
                    appDb.TenantComplianceSnapshots.Add(existingSnapshot);
                }
                else
                {
                    existingSnapshot.TenantName = tenantName;
                    existingSnapshot.ErpType = erpConfig.ErpType.ToString();
                    existingSnapshot.ErpTypeName = erpTypeName;
                    existingSnapshot.OverallScore = overallScore;
                    existingSnapshot.Grade = grade;
                    existingSnapshot.GradeLabel = gradeLabel;
                    existingSnapshot.TotalQueriesEvaluated = totalCount;
                    existingSnapshot.CompliantCount = compliant;
                    existingSnapshot.WarningCount = warning;
                    existingSnapshot.CriticalCount = critical;
                    existingSnapshot.IsPromptSynced = isPromptSynced;
                    existingSnapshot.PromptVersionLabel = promptVerLabel;
                    existingSnapshot.PromptSyncDetails = promptSyncDetails;
                    existingSnapshot.TopViolationsJson = topViolationsJson;
                    existingSnapshot.LastEvaluatedAt = DateTime.UtcNow;
                    appDb.TenantComplianceSnapshots.Update(existingSnapshot);
                }

                await appDb.SaveChangesAsync();
                return resultScorecard;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating Tenant Compliance Scorecard: {ex.Message}");
                return new TenantComplianceScorecard
                {
                    TenantId = tenantId,
                    TenantName = "Tenant",
                    OverallScore = 100,
                    Grade = "A+",
                    GradeLabel = "Hesaplanamadı",
                    PromptSyncDetails = ex.Message
                };
            }
        }

        public async Task<Dictionary<string, TenantComplianceSnapshot>> GetAllTenantComplianceSnapshotsAsync()
        {
            try
            {
                await using var appDb = await _appDbFactory.CreateDbContextAsync();
                await appDb.EnsureTablesCreatedAsync();

                var list = await appDb.TenantComplianceSnapshots.AsNoTracking().ToListAsync();
                return list.ToDictionary(s => s.TenantId, s => s);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all tenant compliance snapshots: {ex.Message}");
                return new Dictionary<string, TenantComplianceSnapshot>();
            }
        }

        private TenantComplianceScorecard GetDemoComplianceScorecard(string tenantId)
        {
            return new TenantComplianceScorecard
            {
                TenantId = tenantId,
                TenantName = "Demo Test Firması A.Ş.",
                ErpType = "Mikro",
                ErpTypeName = "Mikro ERP (v1.0)",
                OverallScore = 86,
                Grade = "A",
                GradeLabel = "Çok İyi",
                TotalQueriesEvaluated = 17,
                CompliantCount = 13,
                WarningCount = 3,
                CriticalCount = 1,
                IsPromptSynced = true,
                PromptVersionLabel = "Mikro v1.0 Senkronize",
                PromptSyncDetails = "Tenant asistan promptu ve veritabanı şeması OzBi Mikro ERP v1.0 güncel standartlarıyla %100 senkronize.",
                TopViolations = new List<TenantRuleViolationStat>
                {
                    new TenantRuleViolationStat
                    {
                        RuleId = "M-NOLOCK",
                        Title = "WITH (NOLOCK) İpucu Eksik",
                        Severity = "Warning",
                        Count = 3,
                        Percentage = 17.6,
                        TotalPenaltyPoints = 15,
                        RecommendedFix = "FROM STOKLAR WITH (NOLOCK) şeklinde lock önleyici ipucu ekleyin.",
                        V26RuleReference = "Kural 1.2: NOLOCK Zorunluluğu"
                    },
                    new TenantRuleViolationStat
                    {
                        RuleId = "M-SOM-RECNO",
                        Title = "STOK_HAREKETLERI som_recno filtresi eksik",
                        Severity = "Error",
                        Count = 1,
                        Percentage = 5.9,
                        TotalPenaltyPoints = 10,
                        RecommendedFix = "WHERE sth_som_recno = 0 koşulunu ekleyin.",
                        V26RuleReference = "Kural 3.1: SOM Kayıt Filtresi"
                    }
                },
                LastEvaluatedAt = DateTime.UtcNow
            };
        }
        #endregion

        #region Demo Data Generator
        private List<TenantAuditSummary> GetDemoTenantsSummary(string? searchTerm = null)
        {
            var list = new List<TenantAuditSummary>
            {
                new TenantAuditSummary
                {
                    Tenant = new OzBiTenant { Id = "demo-tenant-1", Name = "Test Bilişim A.Ş.", Email = "wohovo9001@lasttea.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-30) },
                    TotalChats = 7,
                    TotalMessages = 45,
                    TotalQueries = 17,
                    TotalLogins = 32,
                    LastActivityDate = DateTime.Now.AddHours(-2),
                    IsFavorited = true
                },
                new TenantAuditSummary
                {
                    Tenant = new OzBiTenant { Id = "demo-tenant-2", Name = "Tarık İnşaat San. Ltd.", Email = "Afssd@gmail.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-20) },
                    TotalChats = 1,
                    TotalMessages = 6,
                    TotalQueries = 2,
                    TotalLogins = 5,
                    LastActivityDate = DateTime.Now.AddDays(-10),
                    IsFavorited = false
                },
                new TenantAuditSummary
                {
                    Tenant = new OzBiTenant { Id = "demo-tenant-3", Name = "AYDIN Tekstil A.Ş.", Email = "feritaydin@yilmazsunger.com.tr", IsActive = true, DateCreated = DateTime.Now.AddDays(-15) },
                    TotalChats = 4,
                    TotalMessages = 28,
                    TotalQueries = 13,
                    TotalLogins = 18,
                    LastActivityDate = DateTime.Now.AddDays(-15),
                    IsFavorited = true
                }
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                list = list.Where(t => t.Tenant.Name.ToLower().Contains(term) || t.Tenant.Email?.ToLower().Contains(term) == true).ToList();
            }

            return list;
        }

        private List<UserAuditSummary> GetDemoUsersSummary(string? searchTerm = null)
        {
            var list = new List<UserAuditSummary>
            {
                new UserAuditSummary
                {
                    User = new OzBiUser { Id = "demo-usr-1", NameSurname = "Ahmet Yılmaz", Email = "ahmet@testbilisim.com", TenantId = "demo-tenant-1", IsActive = true },
                    TenantName = "Test Bilişim A.Ş.",
                    TotalChats = 5,
                    TotalMessages = 32,
                    TotalQueries = 12,
                    LastActivityDate = DateTime.Now.AddHours(-1),
                    IsFavorited = true
                },
                new UserAuditSummary
                {
                    User = new OzBiUser { Id = "demo-usr-2", NameSurname = "Ferit Aydın", Email = "feritaydin@yilmazsunger.com.tr", TenantId = "demo-tenant-3", IsActive = true },
                    TenantName = "AYDIN Tekstil A.Ş.",
                    TotalChats = 4,
                    TotalMessages = 28,
                    TotalQueries = 13,
                    LastActivityDate = DateTime.Now.AddDays(-2),
                    IsFavorited = true
                }
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                list = list.Where(u => u.User.NameSurname?.ToLower().Contains(term) == true || u.User.Email?.ToLower().Contains(term) == true).ToList();
            }

            return list;
        }

        private List<ChatAuditSummary> GetDemoChatsForTenant(string tenantId, string? searchTerm = null)
        {
            return new List<ChatAuditSummary>
            {
                new ChatAuditSummary
                {
                    Chat = new OzBiChat { Id = "demo-chat-101", Title = "Son 30 Gün Ödeme Yapmayan Cariler", TenantId = tenantId, IsActive = true, DateCreated = DateTime.Now.AddHours(-2) },
                    MessageCount = 6,
                    QueryCount = 3,
                    TotalDurationMs = 1240,
                    LastMessageDate = DateTime.Now.AddHours(-2),
                    PrimaryAiModelName = "GPT-4o"
                },
                new ChatAuditSummary
                {
                    Chat = new OzBiChat { Id = "demo-chat-102", Title = "Aylık Stok & Satış Raporu Özeti", TenantId = tenantId, IsActive = true, DateCreated = DateTime.Now.AddDays(-1) },
                    MessageCount = 4,
                    QueryCount = 2,
                    TotalDurationMs = 890,
                    LastMessageDate = DateTime.Now.AddDays(-1),
                    PrimaryAiModelName = "Claude 3.5 Sonnet"
                }
            };
        }

        private List<OzBiChatMessage> GetDemoMessagesForChat(string chatId)
        {
            return new List<OzBiChatMessage>
            {
                new OzBiChatMessage
                {
                    Id = "demo-msg-1",
                    ChatId = chatId,
                    Role = "user",
                    Message = "Son 30 günde ödeme yapmayan carileri ve bakiyelerini listeleyebilir misin?",
                    DateCreated = DateTime.Now.AddHours(-2)
                },
                new OzBiChatMessage
                {
                    Id = "demo-msg-2",
                    ChatId = chatId,
                    Role = "assistant",
                    Message = "Elbette, Mikro ERP veritabanınızdan son 30 günde tahsilat kaydı bulunmayan carilerin sorgusu çalıştırıldı ve sonuçlar listelendi.",
                    Query = "SELECT cha_kod AS CariKod, cha_isim AS CariUnvan, SUM(cha_meblag) AS ToplamBakiye\nFROM CARI_HESAP_HAREKETLERI\nWHERE cha_tarih >= DATEADD(month, -1, GETDATE()) AND cha_evrak_tip NOT IN (1, 3)\nGROUP BY cha_kod, cha_isim\nHAVING SUM(cha_meblag) > 0;",
                    IsSucceeded = true,
                    TotalDurationMs = 450,
                    AIQueryDurationMs = 320,
                    AIModel = new OzBiAiModel { Name = "GPT-4o", TokenLimit = 128000 },
                    Assistant = new OzBiAssistant { Name = "Mikro ERP Muhasebe Asistanı" },
                    DateCreated = DateTime.Now.AddHours(-2).AddSeconds(2)
                }
            };
        }

        private List<OzBiChatMessage> GetDemoGlobalQueries(string? querySearch, bool? failedOnly, long? minDurationMs)
        {
            var chat1 = new OzBiChat { Id = "demo-chat-101", Title = "Şirketin 30 Günlük Nakit Akış Projeksiyonu", TenantId = "demo-tenant-1", Tenant = new OzBiTenant { Name = "ozbidemo" }, CreatedByUser = new OzBiUser { NameSurname = "Eren Çülcüoğlu" } };

            var list = new List<OzBiChatMessage>
            {
                new OzBiChatMessage
                {
                    Id = "demo-msg-101-1",
                    ChatId = chat1.Id,
                    Chat = chat1,
                    IsAsistantMode = true,
                    Prompt = "Şirketin 30 günlük nakit akış projeksiyonunu çıkarır mısın? Kasadaki ve bankadaki nakit, vadesi 30 gün içinde gelecek alacak çekleri, bekleyen açık cari alacaklar EKSİ vadesi gelecek cari borçlar ve kredi taksitleri şeklinde hesapla.",
                    Query = @"[
                        {
                            ""description"": ""Son 12 ay için aylık satış geliri ve sipariş sayısı trendi"",
                            ""sql"": ""SELECT DATE_FORMAT(`order_date`, '%Y-%m') AS `Ay`, SUM(`total_amount`) AS `SatisGeliriUSD`, COUNT(`id`) AS `SiparisSayisi` FROM `erp_orders` WHERE `order_date` >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH) AND `order_date` < DATE_ADD(LAST_DAY(CURDATE()), INTERVAL 1 DAY) GROUP BY DATE_FORMAT(`order_date`, '%Y-%m') ORDER BY `Ay`;""
                        }
                    ]",
                    TotalDurationMs = 47709,
                    AIModel = new OzBiAiModel { Name = "GPT-5.6 Terra" },
                    Assistant = new OzBiAssistant { Name = "ERP Assistant - New" },
                    DateCreated = DateTime.Now.AddMinutes(-20)
                },
                new OzBiChatMessage
                {
                    Id = "demo-msg-101-2",
                    ChatId = chat1.Id,
                    Chat = chat1,
                    IsAsistantMode = true,
                    Prompt = "Şirketin 30 günlük nakit akış projeksiyonunu çıkarır mısın? Kasadaki ve bankadaki nakit, vadesi 30 gün içinde gelecek alacak çekleri, bekleyen açık cari alacaklar EKSİ vadesi gelecek cari borçlar ve kredi taksitleri şeklinde hesapla.",
                    Query = @"[
                        {
                            ""description"": ""Vadesi geçmiş ödenmemiş faturaların toplam alacak tutarı (USD)."",
                            ""sql"": ""SELECT SUM(`final_amount`) AS `total_overdue_receivables_usd` FROM `erp_invoices` WHERE `is_paid` = 0 AND `due_date` < CURRENT_DATE;""
                        },
                        {
                            ""description"": ""Toplam vadesi geçmiş borca göre en borçlu 5 müşterinin gecikme yaşlandırma dilimlerine göre alacak tutarları (USD)."",
                            ""sql"": ""SELECT `c`.`company_name` AS `customer`, CASE WHEN DATEDIFF(CURRENT_DATE, `i`.`due_date`) <= 30 THEN '0-30 gün' WHEN DATEDIFF(CURRENT_DATE, `i`.`due_date`) <= 60 THEN '30-60 gün' ELSE '60+ gün' END AS `overdue_days_bucket`, SUM(`i`.`final_amount`) AS `overdue_amount_usd` FROM `erp_invoices` AS `i` JOIN `erp_orders` AS `o` ON `i`.`order_id` = `o`.`id` JOIN `erp_customers` AS `c` ON `o`.`customer_id` = `c`.`id` WHERE `i`.`is_paid` = 0 GROUP BY `c`.`company_name`, `overdue_days_bucket` ORDER BY `overdue_amount_usd` DESC LIMIT 5;""
                        }
                    ]",
                    TotalDurationMs = 34625,
                    AIModel = new OzBiAiModel { Name = "GPT-5.6 Terra" },
                    Assistant = new OzBiAssistant { Name = "ERP Assistant - New" },
                    DateCreated = DateTime.Now.AddMinutes(-35)
                },
                new OzBiChatMessage
                {
                    Id = "demo-msg-101-3",
                    ChatId = chat1.Id,
                    Chat = chat1,
                    IsAsistantMode = true,
                    Prompt = "Şirketin 30 günlük nakit akış projeksiyonunu çıkarır mısın? Kasadaki ve bankadaki nakit, vadesi 30 gün içinde gelecek alacak çekleri, bekleyen açık cari alacaklar EKSİ vadesi gelecek cari borçlar ve kredi taksitleri şeklinde hesapla.",
                    Query = @"[
                        {
                            ""description"": ""Vadesi geçmiş ve ödenmemiş tedarikçi faturalarının tedarikçi bazında adet ve tutar dağılımı"",
                            ""sql"": ""SELECT `v`.`vendor_name`, `v`.`vendor_category`, COUNT(`e`.`expense_id`) AS `overdue_invoice_count`, SUM(`e`.`amount_usd`) AS `overdue_amount_usd` FROM `mitas_expenses` AS `e` INNER JOIN `mitas_vendors` AS `v` ON `v`.`vendor_id` = `e`.`vendor_id` WHERE `e`.`due_date` < CURDATE() GROUP BY `v`.`vendor_id`, `v`.`vendor_name`, `v`.`vendor_category`;""
                        }
                    ]",
                    TotalDurationMs = 28140,
                    AIModel = new OzBiAiModel { Name = "GPT-5.6 Terra" },
                    Assistant = new OzBiAssistant { Name = "ERP Assistant - New" },
                    DateCreated = DateTime.Now.AddMinutes(-50)
                }
            };

            return list;
        }

        #region Demo Feedbacks
        private List<OzBiChatMessage> GetDemoCustomerFeedbacks(string? tenantId, string? searchTerm, string? filterType, int maxResults)
        {
            var demoTenant1 = new OzBiTenant { Id = "demo-tenant-1", Name = "Test Bilişim A.Ş." };
            var demoTenant2 = new OzBiTenant { Id = "demo-tenant-2", Name = "Tarık İnşaat San. Ltd." };
            var demoUser1 = new OzBiUser { Id = "user-1", NameSurname = "Eren Çülcüoğlu", Email = "eren@ozbi.com" };
            var demoUser2 = new OzBiUser { Id = "user-2", NameSurname = "Mehmet Uşma", Email = "mehmet@usma.com" };

            var chat1 = new OzBiChat { Id = "chat-fb-1", Title = "Kritik düşük veya stokta yok durumundaki ürünleri listele", TenantId = demoTenant1.Id, Tenant = demoTenant1, CreatedByUser = demoUser1 };
            var chat2 = new OzBiChat { Id = "chat-fb-2", Title = "Vadesi geçen alacak çekleri ve müşteri yaşlandırma tablosu", TenantId = demoTenant2.Id, Tenant = demoTenant2, CreatedByUser = demoUser2 };
            var chat3 = new OzBiChat { Id = "chat-fb-3", Title = "Bu ay en çok tahsilat yapılan cariler", TenantId = demoTenant1.Id, Tenant = demoTenant1, CreatedByUser = demoUser1 };

            var list = new List<OzBiChatMessage>
            {
                new OzBiChatMessage
                {
                    Id = "fb-msg-1",
                    ChatId = chat1.Id,
                    Chat = chat1,
                    Role = "Model",
                    IsLiked = false,
                    FeedbackReason = "Tablodaki stok miktarları ile Logo ERP ana ekranı uyuşmuyor, rezerve stoklar düşülmemiş.",
                    Prompt = "Kritik düşük veya stokta yok durumundaki ürünleri listele",
                    Message = "## Kritik Stok Durumu Listesi\n\n🔴 **Stokta Yok Ürün:** 1 ürün\n⚠️ **Kritik Düşük Ürün:** 21 ürün\n\nToplam 22 ürün için acil tedarik planlaması gerekmektedir.",
                    Query = @"[{""description"":""Kritik seviyenin altındaki stok kartları ve ambar bakiyeleri"",""sql"":""SELECT s.CODE AS StokKodu, s.NAME AS StokAdi, SUM(g.ONHAND) AS FiiliStok FROM LG_001_ITEMS s WITH (NOLOCK) LEFT JOIN LV_001_01_GNTOTST g WITH (NOLOCK) ON g.STOCKREF = s.LOGICALREF WHERE g.INVENNO = 0 GROUP BY s.CODE, s.NAME HAVING SUM(g.ONHAND) <= 15 ORDER BY FiiliStok ASC;""}]",
                    IsSucceeded = true,
                    TotalDurationMs = 19450,
                    AIModel = new OzBiAiModel { Name = "GPT-5.4 mini" },
                    Assistant = new OzBiAssistant { Name = "ERP Assistant - Logo v7.2" },
                    DateCreated = DateTime.Now.AddHours(-3)
                },
                new OzBiChatMessage
                {
                    Id = "fb-msg-2",
                    ChatId = chat2.Id,
                    Chat = chat2,
                    Role = "Model",
                    IsLiked = false,
                    FeedbackReason = "Sorgu Mikro v27 veritabanında Timeout hatası verdi, cha_som_recno filtresi eksik.",
                    Prompt = "Vadesi geçen alacak çekleri ve müşteri yaşlandırma tablosunu getir",
                    Message = null,
                    Query = @"[{""description"":""Gecikmiş çekler ve cari yaşlandırma"",""sql"":""SELECT cari_unvan1, SUM(cek_tutari) AS ToplamTutar FROM CEKLER JOIN CARI_HESAPLAR ON cek_cari_kodu = cari_kod WHERE cek_vade < GETDATE() GROUP BY cari_unvan1;""}]",
                    IsSucceeded = false,
                    ErrorMessage = "SqlException: Execution Timeout Expired. Missing with(nolock) or som_recno index optimization.",
                    TotalDurationMs = 60120,
                    AIModel = new OzBiAiModel { Name = "GPT-5.4 mini" },
                    Assistant = new OzBiAssistant { Name = "ERP Assistant - Mikro v27" },
                    DateCreated = DateTime.Now.AddDays(-1)
                },
                new OzBiChatMessage
                {
                    Id = "fb-msg-3",
                    ChatId = chat3.Id,
                    Chat = chat3,
                    Role = "Model",
                    IsLiked = true,
                    FeedbackReason = null,
                    Prompt = "Bu ay en çok tahsilat yapılan cariler",
                    Message = "## Bu Ay En Çok Tahsilat Yapılan Cariler\n\nToplam tahsilat tutarı **13.168.838,68 TL**’dir. Tahsilatın %64'ü ilk 3 ana müşteriden gerçekleştirilmiştir.",
                    Query = @"[{""description"":""Bu ay tahsilat toplamları"",""sql"":""SELECT cha_kod, SUM(cha_meblag) AS TahsilatTutar FROM CARI_HESAP_HAREKETLERI WITH (NOLOCK) WHERE cha_tip = 1 AND cha_tarihi >= '2026-08-01' AND cha_som_recno = 0 GROUP BY cha_kod ORDER BY TahsilatTutar DESC;""}]",
                    IsSucceeded = true,
                    TotalDurationMs = 12340,
                    AIModel = new OzBiAiModel { Name = "GPT-5.4 mini" },
                    Assistant = new OzBiAssistant { Name = "ERP Assistant" },
                    DateCreated = DateTime.Now.AddDays(-2)
                }
            };

            var query = list.AsQueryable();

            if (filterType == "disliked")
                query = query.Where(m => m.IsLiked == false || (!string.IsNullOrEmpty(m.FeedbackReason)));
            else if (filterType == "comments_only")
                query = query.Where(m => !string.IsNullOrEmpty(m.FeedbackReason));
            else if (filterType == "liked_only")
                query = query.Where(m => m.IsLiked == true);
            else if (filterType == "failed_disliked")
                query = query.Where(m => (m.IsLiked == false || !string.IsNullOrEmpty(m.FeedbackReason)) && (!m.IsSucceeded || !string.IsNullOrEmpty(m.ErrorMessage)));

            if (!string.IsNullOrWhiteSpace(tenantId))
                query = query.Where(m => m.Chat != null && m.Chat.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(m =>
                    (m.FeedbackReason != null && m.FeedbackReason.ToLower().Contains(term)) ||
                    (m.Prompt != null && m.Prompt.ToLower().Contains(term)) ||
                    (m.Message != null && m.Message.ToLower().Contains(term)) ||
                    (m.Query != null && m.Query.ToLower().Contains(term))
                );
            }

            return query.OrderByDescending(m => m.DateCreated).Take(maxResults).ToList();
        }

        private FeedbackAuditStats GetDemoFeedbackStats(string? tenantId)
        {
            return new FeedbackAuditStats
            {
                TotalFeedbacks = 28,
                DislikedCount = 4,
                LikedCount = 24,
                WithCommentCount = 3,
                FailedAndDislikedCount = 1
            };
        }
        #endregion

        #region Customer Feedbacks & Dislike Hub Implementation
        public async Task<List<OzBiChatMessage>> GetCustomerFeedbacksAsync(
            string? tenantId = null,
            string? searchTerm = null,
            string? filterType = "disliked",
            int maxResults = 150)
        {
            if (_useDemoMode) return GetDemoCustomerFeedbacks(tenantId, searchTerm, filterType, maxResults);

            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = db.ChatMessages.AsNoTracking()
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.Tenant)
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.CreatedByUser)
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                    .ThenInclude(a => a!.DataConnection)
                        .ThenInclude(dc => dc!.ConnectionSourceCode)
                .AsQueryable();

            if (filterType == "disliked")
            {
                query = query.Where(m => m.IsLiked == false || (m.FeedbackReason != null && m.FeedbackReason != ""));
            }
            else if (filterType == "comments_only")
            {
                query = query.Where(m => m.FeedbackReason != null && m.FeedbackReason != "");
            }
            else if (filterType == "liked_only")
            {
                query = query.Where(m => m.IsLiked == true);
            }
            else if (filterType == "failed_disliked")
            {
                query = query.Where(m => (m.IsLiked == false || (m.FeedbackReason != null && m.FeedbackReason != "")) && (m.ErrorMessage != null && m.ErrorMessage != ""));
            }
            else
            {
                // All with feedback
                query = query.Where(m => m.IsLiked != null || (m.FeedbackReason != null && m.FeedbackReason != ""));
            }

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                query = query.Where(m => m.Chat != null && m.Chat.TenantId == tenantId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(m =>
                    (m.FeedbackReason != null && m.FeedbackReason.ToLower().Contains(term)) ||
                    (m.Prompt != null && m.Prompt.ToLower().Contains(term)) ||
                    (m.Message != null && m.Message.ToLower().Contains(term)) ||
                    (m.Query != null && m.Query.ToLower().Contains(term)) ||
                    (m.ErrorMessage != null && m.ErrorMessage.ToLower().Contains(term)) ||
                    (m.Chat != null && m.Chat.Title != null && m.Chat.Title.ToLower().Contains(term))
                );
            }

            var messages = await query
                .OrderByDescending(m => m.DateCreated)
                .Take(maxResults)
                .ToListAsync();

            var chatIds = messages.Select(m => m.ChatId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();

            if (chatIds.Count > 0)
            {
                var userMessages = await db.ChatMessages.AsNoTracking()
                    .Where(m => chatIds.Contains(m.ChatId) && (m.Role == "user" || m.Role == "User"))
                    .Select(m => new { m.ChatId, m.Message, m.Prompt, m.DateCreated })
                    .ToListAsync();

                foreach (var msg in messages)
                {
                    if (string.IsNullOrWhiteSpace(msg.Prompt))
                    {
                        var userPrompt = userMessages
                            .Where(u => u.ChatId == msg.ChatId && u.DateCreated <= msg.DateCreated)
                            .OrderByDescending(u => u.DateCreated)
                            .FirstOrDefault();

                        if (userPrompt != null)
                        {
                            msg.Prompt = !string.IsNullOrWhiteSpace(userPrompt.Message) ? userPrompt.Message : userPrompt.Prompt;
                        }
                    }
                }
            }

            return messages;
        }

        public async Task<FeedbackAuditStats> GetFeedbackStatsAsync(string? tenantId = null)
        {
            if (_useDemoMode) return GetDemoFeedbackStats(tenantId);

            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = db.ChatMessages.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                query = query.Where(m => m.Chat != null && m.Chat.TenantId == tenantId);
            }

            var feedbacks = await query
                .Where(m => m.IsLiked != null || (m.FeedbackReason != null && m.FeedbackReason != ""))
                .Select(m => new { m.IsLiked, m.FeedbackReason, m.IsSucceeded, m.ErrorMessage })
                .ToListAsync();

            return new FeedbackAuditStats
            {
                TotalFeedbacks = feedbacks.Count,
                DislikedCount = feedbacks.Count(f => f.IsLiked == false),
                LikedCount = feedbacks.Count(f => f.IsLiked == true),
                WithCommentCount = feedbacks.Count(f => !string.IsNullOrEmpty(f.FeedbackReason)),
                FailedAndDislikedCount = feedbacks.Count(f => f.IsLiked == false && !string.IsNullOrEmpty(f.ErrorMessage))
            };
        }

        public async Task<List<OzBiAssistant>> GetAssistantsForTenantAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) return new List<OzBiAssistant>();

            if (_useDemoMode)
            {
                return new List<OzBiAssistant>
                {
                    new OzBiAssistant
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Logo ERP - Asistan",
                        Description = "Logo Tiger / Go ERP Entegrasyon Asistanı",
                        AIModelName = "GPT-5.6 Terra",
                        SubModel = "gemini-2.5-flash",
                        Temperature = 0.1,
                        TopP = 0.95,
                        TenantId = tenantId,
                        IsActive = true,
                        DateCreated = DateTime.UtcNow.AddMonths(-1),
                        DateModified = DateTime.UtcNow.AddDays(-2),
                        DatabaseDefinition = "[{\"TABLE_NAME\":\"LG_001_CLCARD\",\"Type\":\"TABLE\",\"Description\":\"Cari Hesap Kartları\",\"Columns\":[{\"Name\":\"LOGICALREF\",\"Type\":\"int\"},{\"Name\":\"CODE\",\"Type\":\"varchar\"},{\"Name\":\"DEFINITION_\",\"Type\":\"varchar\"}]}]",
                        UserAdditionalPrompt = "# OzBI Logo ERP Ek Talimatı — v8.0\n\nPozitif kurallar...",
                        UserAdditionalAgentPrompt = "# OzBI Kurumsal Analist Ek Prompt Yönergesi\n\nAnaliz kuralları..."
                    }
                };
            }

            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();
                var assistants = await db.Assistants.AsNoTracking()
                    .Include(a => a.DataConnection)
                    .Where(a => a.TenantId == tenantId)
                    .OrderByDescending(a => a.IsActive)
                    .ThenByDescending(a => a.DateModified)
                    .ToListAsync();

                return assistants;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OzBiAuditService] Error fetching assistants for tenant {tenantId}: {ex.Message}");
                return new List<OzBiAssistant>();
            }
        }
        #endregion
        #endregion
    }
}

