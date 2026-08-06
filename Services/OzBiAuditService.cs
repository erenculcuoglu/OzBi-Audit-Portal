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

        public OzBiAuditService(IDbContextFactory<OzBiDbContext> dbFactory, IDbContextFactory<AppDbContext> appDbFactory)
        {
            _dbFactory = dbFactory;
            _appDbFactory = appDbFactory;
        }

        public async Task<List<TenantAuditSummary>> GetTenantsSummaryAsync(string? searchTerm = null, int portalUserId = 0)
        {
            if (_useDemoMode)
            {
                return GetDemoTenantsSummary(searchTerm);
            }

            try
            {
                using var db = await _dbFactory.CreateDbContextAsync();

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
                using var db = await _dbFactory.CreateDbContextAsync();

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

            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<List<ChatAuditSummary>> GetChatsForTenantAsync(string tenantId, string? searchTerm = null, string? filterUserId = null)
        {
            if (_useDemoMode) return GetDemoChatsForTenant(tenantId, searchTerm);

            using var db = await _dbFactory.CreateDbContextAsync();

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

                var userQuestions = msgList
                    .Where(m => m.Role?.ToLower() == "user" || !string.IsNullOrEmpty(m.Prompt) || (!string.IsNullOrEmpty(m.Message) && !m.Message.StartsWith("##") && !m.Message.StartsWith("SELECT") && !m.Message.StartsWith("|")))
                    .Select(m => !string.IsNullOrWhiteSpace(m.Message) && m.Role?.ToLower() == "user" ? m.Message : m.Prompt)
                    .Where(q => !string.IsNullOrWhiteSpace(q))
                    .Select(q => q!.Trim())
                    .Distinct()
                    .ToList();

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

                var userQuestionCount = userQuestions.Count > 0 ? userQuestions.Count : msgList.Count(m => m.Role?.ToLower() == "user" || !string.IsNullOrEmpty(m.Prompt));
                if (userQuestionCount == 0 && msgList.Count > 0) userQuestionCount = msgList.Count;

                bool isAssistantMode = msgList.Any(m => m.IsAsistantMode);

                result.Add(new ChatAuditSummary
                {
                    Chat = c,
                    MessageCount = userQuestionCount,
                    QueryCount = msgList.Count(m => !string.IsNullOrEmpty(m.Query)),
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

            using var db = await _dbFactory.CreateDbContextAsync();

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

            using var db = await _dbFactory.CreateDbContextAsync();

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

                var userQuestions = msgList
                    .Where(m => m.Role?.ToLower() == "user" || !string.IsNullOrEmpty(m.Prompt) || (!string.IsNullOrEmpty(m.Message) && !m.Message.StartsWith("##") && !m.Message.StartsWith("SELECT") && !m.Message.StartsWith("|")))
                    .Select(m => !string.IsNullOrWhiteSpace(m.Message) && m.Role?.ToLower() == "user" ? m.Message : m.Prompt)
                    .Where(q => !string.IsNullOrWhiteSpace(q))
                    .Select(q => q!.Trim())
                    .Distinct()
                    .ToList();

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

                var userQuestionCount = userQuestions.Count > 0 ? userQuestions.Count : msgList.Count(m => m.Role?.ToLower() == "user" || !string.IsNullOrEmpty(m.Prompt));
                if (userQuestionCount == 0 && msgList.Count > 0) userQuestionCount = msgList.Count;

                bool isAssistantMode = msgList.Any(m => m.IsAsistantMode);

                result.Add(new ChatAuditSummary
                {
                    Chat = c,
                    MessageCount = userQuestionCount,
                    QueryCount = msgList.Count(m => !string.IsNullOrEmpty(m.Query)),
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

        public async Task<OzBiChat?> GetChatByIdAsync(string chatId)
        {
            if (_useDemoMode) return new OzBiChat { Id = chatId, Title = "Demo Analiz Sohbeti", TenantId = "demo-tenant-1", DateCreated = DateTime.Now.AddDays(-1) };

            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Chats.AsNoTracking()
                .Include(c => c.Tenant)
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<List<OzBiChatMessage>> GetMessagesForChatAsync(string chatId)
        {
            if (_useDemoMode) return GetDemoMessagesForChat(chatId);

            using var db = await _dbFactory.CreateDbContextAsync();
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

            using var db = await _dbFactory.CreateDbContextAsync();

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

            return await query.OrderByDescending(m => m.DateCreated).Take(maxResults).ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetAiModelUsageStatsAsync()
        {
            if (_useDemoMode) return new Dictionary<string, int> { { "GPT-4o", 145 }, { "Claude 3.5 Sonnet", 98 }, { "DeepSeek V3", 42 } };

            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.ChatMessages.AsNoTracking()
                .Where(m => m.AIModel != null && m.AIModel.Name != null)
                .GroupBy(m => m.AIModel!.Name!)
                .Select(g => new { ModelName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ModelName, x => x.Count);
        }

        #region Favorite & Footprint Persistence Methods
        public async Task<bool> ToggleFavoriteAsync(int portalUserId, string itemType, string itemId, string itemName, string? itemSubText)
        {
            using var db = await _appDbFactory.CreateDbContextAsync();
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
            using var db = await _appDbFactory.CreateDbContextAsync();
            await db.EnsureTablesCreatedAsync();
            return await db.Favorites.AsNoTracking()
                .Where(f => f.PortalUserId == portalUserId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();
        }

        public async Task<HashSet<string>> GetFavoriteItemIdsAsync(int portalUserId, string itemType)
        {
            using var db = await _appDbFactory.CreateDbContextAsync();
            await db.EnsureTablesCreatedAsync();
            var ids = await db.Favorites.AsNoTracking()
                .Where(f => f.PortalUserId == portalUserId && f.ItemType == itemType)
                .Select(f => f.ItemId)
                .ToListAsync();
            return ids.ToHashSet();
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
            var msgs = GetDemoMessagesForChat("demo-chat-101");
            msgs[1].Chat = new OzBiChat { Id = "demo-chat-101", Title = "Son 30 Gün Ödeme Yapmayan Cariler", TenantId = "demo-tenant-1" };
            return msgs;
        }
        #endregion
    }
}
