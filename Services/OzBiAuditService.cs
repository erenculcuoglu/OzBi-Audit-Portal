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
        private static bool _useDemoMode = false;

        public static bool UseDemoMode
        {
            get => _useDemoMode;
            set => _useDemoMode = value;
        }

        public OzBiAuditService(IDbContextFactory<OzBiDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<TenantAuditSummary>> GetTenantsSummaryAsync(string? searchTerm = null)
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
                                             t.Id.ToLower().Contains(term));
                }

                var tenants = await query.OrderByDescending(t => t.DateCreated).Take(200).ToListAsync();
                var tenantIds = tenants.Select(t => t.Id).ToHashSet();

                var chats = await db.Chats.AsNoTracking()
                    .Where(c => tenantIds.Contains(c.TenantId))
                    .Select(c => new { c.Id, c.TenantId, c.DateCreated })
                    .ToListAsync();

                var chatIds = chats.Select(c => c.Id).ToHashSet();

                var messages = await db.ChatMessages.AsNoTracking()
                    .Where(m => chatIds.Contains(m.ChatId))
                    .Select(m => new { m.ChatId, HasQuery = !string.IsNullOrEmpty(m.Query) })
                    .ToListAsync();

                var result = new List<TenantAuditSummary>();

                foreach (var t in tenants)
                {
                    var tenantChats = chats.Where(c => c.TenantId == t.Id).ToList();
                    var tenantChatIds = tenantChats.Select(c => c.Id).ToHashSet();
                    var tenantMessages = messages.Where(m => tenantChatIds.Contains(m.ChatId)).ToList();

                    result.Add(new TenantAuditSummary
                    {
                        Tenant = t,
                        TotalChats = tenantChats.Count,
                        TotalMessages = tenantMessages.Count,
                        TotalQueries = tenantMessages.Count(m => m.HasQuery),
                        LastActivityDate = tenantChats.Count > 0 ? tenantChats.Max(c => c.DateCreated) : t.DateCreated
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"MySQL Veritabanı Sorgu Hatası: {ex.Message}", ex);
            }
        }

        public async Task<OzBiTenant?> GetTenantByIdAsync(string tenantId)
        {
            if (_useDemoMode)
            {
                return GetDemoTenants().FirstOrDefault(t => t.Id == tenantId);
            }

            try
            {
                using var db = await _dbFactory.CreateDbContextAsync();
                return await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
            }
            catch
            {
                return GetDemoTenants().FirstOrDefault(t => t.Id == tenantId);
            }
        }

        public async Task<List<ChatAuditSummary>> GetChatsForTenantAsync(string tenantId, string? searchTerm = null)
        {
            if (_useDemoMode)
            {
                return GetDemoChatsSummary(tenantId, searchTerm);
            }

            try
            {
                using var db = await _dbFactory.CreateDbContextAsync();

                var query = db.Chats.AsNoTracking()
                    .Include(c => c.CreatedByUser)
                    .Where(c => c.TenantId == tenantId);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(c => (c.Title != null && c.Title.ToLower().Contains(term)) ||
                                             c.Id.ToLower().Contains(term));
                }

                var chats = await query.OrderByDescending(c => c.DateCreated).Take(200).ToListAsync();
                var chatIds = chats.Select(c => c.Id).ToList();

                var msgStats = await db.ChatMessages.AsNoTracking()
                    .Where(m => chatIds.Contains(m.ChatId))
                    .GroupBy(m => m.ChatId)
                    .Select(g => new
                    {
                        ChatId = g.Key,
                        Count = g.Count(),
                        QueryCount = g.Count(m => !string.IsNullOrEmpty(m.Query)),
                        TotalDuration = g.Sum(m => m.TotalDurationMs ?? 0),
                        LastDate = g.Max(m => m.DateCreated),
                        AiModelId = g.Where(m => m.AIModelId != null).Select(m => m.AIModelId).FirstOrDefault()
                    })
                    .ToDictionaryAsync(x => x.ChatId);

                var modelIds = msgStats.Values.Where(v => v.AiModelId != null).Select(v => v.AiModelId!).Distinct().ToList();
                var modelMap = await db.AiModels.AsNoTracking()
                    .Where(m => modelIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Name ?? m.ProgrammaticName ?? "Bilinmeyen Model");

                var result = new List<ChatAuditSummary>();
                foreach (var chat in chats)
                {
                    msgStats.TryGetValue(chat.Id, out var stat);
                    string? modelName = null;
                    if (stat?.AiModelId != null && modelMap.TryGetValue(stat.AiModelId, out var mName))
                    {
                        modelName = mName;
                    }

                    result.Add(new ChatAuditSummary
                    {
                        Chat = chat,
                        MessageCount = stat?.Count ?? 0,
                        QueryCount = stat?.QueryCount ?? 0,
                        TotalDurationMs = stat?.TotalDuration ?? 0,
                        LastMessageDate = stat?.LastDate ?? chat.DateCreated,
                        PrimaryAiModelName = modelName
                    });
                }

                return result;
            }
            catch
            {
                return GetDemoChatsSummary(tenantId, searchTerm);
            }
        }

        public async Task<OzBiChat?> GetChatByIdAsync(string chatId)
        {
            if (_useDemoMode)
            {
                return GetDemoChats().FirstOrDefault(c => c.Id == chatId);
            }

            try
            {
                using var db = await _dbFactory.CreateDbContextAsync();
                return await db.Chats.AsNoTracking()
                    .Include(c => c.Tenant)
                    .Include(c => c.CreatedByUser)
                    .FirstOrDefaultAsync(c => c.Id == chatId);
            }
            catch
            {
                return GetDemoChats().FirstOrDefault(c => c.Id == chatId);
            }
        }

        public async Task<List<OzBiChatMessage>> GetMessagesForChatAsync(string chatId)
        {
            if (_useDemoMode)
            {
                return GetDemoMessages(chatId);
            }

            try
            {
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
            catch
            {
                return GetDemoMessages(chatId);
            }
        }

        public async Task<List<OzBiChatMessage>> SearchGlobalQueriesAsync(
            string? tenantId = null,
            string? querySearch = null,
            bool? failedOnly = false,
            long? minDurationMs = null,
            int maxResults = 100)
        {
            if (_useDemoMode)
            {
                return GetDemoGlobalQueries(querySearch, failedOnly, minDurationMs);
            }

            try
            {
                using var db = await _dbFactory.CreateDbContextAsync();

                var query = db.ChatMessages.AsNoTracking()
                    .Include(m => m.Chat)
                    .ThenInclude(c => c!.Tenant)
                    .Include(m => m.AIModel)
                    .Where(m => !string.IsNullOrEmpty(m.Query));

                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    query = query.Where(m => m.Chat != null && m.Chat.TenantId == tenantId);
                }

                if (!string.IsNullOrWhiteSpace(querySearch))
                {
                    var term = querySearch.Trim().ToLower();
                    query = query.Where(m => (m.Query != null && m.Query.ToLower().Contains(term)) ||
                                             (m.Prompt != null && m.Prompt.ToLower().Contains(term)) ||
                                             (m.Message != null && m.Message.ToLower().Contains(term)));
                }

                if (failedOnly == true)
                {
                    query = query.Where(m => !m.IsSucceeded || !string.IsNullOrEmpty(m.ErrorMessage));
                }

                if (minDurationMs.HasValue && minDurationMs.Value > 0)
                {
                    query = query.Where(m => m.TotalDurationMs >= minDurationMs.Value);
                }

                return await query.OrderByDescending(m => m.DateCreated)
                    .Take(maxResults)
                    .ToListAsync();
            }
            catch
            {
                return GetDemoGlobalQueries(querySearch, failedOnly, minDurationMs);
            }
        }

        public async Task<Dictionary<string, int>> GetAiModelUsageStatsAsync()
        {
            return new Dictionary<string, int>
            {
                { "GPT-4o (OpenAI)", 45 },
                { "Claude 3.5 Sonnet (Anthropic)", 32 },
                { "Gemini 1.5 Pro (Google)", 18 }
            };
        }

        #region Demo Data Generator
        private List<OzBiTenant> GetDemoTenants()
        {
            return new List<OzBiTenant>
            {
                new OzBiTenant { Id = "tenant-001", Name = "Test Bilişim A.Ş.", Email = "wohovo9001@lasttea.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-10) },
                new OzBiTenant { Id = "tenant-002", Name = "PARKİM Ambalaj Sanayi", Email = "info@parkim.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-24) },
                new OzBiTenant { Id = "tenant-003", Name = "Çözüm Bilgisayar Yazılım", Email = "destek@cozumbilgisayar.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-23) },
                new OzBiTenant { Id = "tenant-004", Name = "EGE TABAN Sanayi ve Ticaret", Email = "muhasebe@egetaban.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-28) },
                new OzBiTenant { Id = "tenant-005", Name = "In70Coffee Gıda Şti.", Email = "in70coffee@gmail.com", IsActive = true, DateCreated = DateTime.Now.AddDays(-28) }
            };
        }

        private List<TenantAuditSummary> GetDemoTenantsSummary(string? searchTerm)
        {
            var tenants = GetDemoTenants();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                tenants = tenants.Where(t => t.Name.ToLower().Contains(term) || (t.Email != null && t.Email.ToLower().Contains(term)) || t.Id.ToLower().Contains(term)).ToList();
            }

            return tenants.Select(t => new TenantAuditSummary
            {
                Tenant = t,
                TotalChats = t.Id == "tenant-001" ? 8 : 4,
                TotalMessages = t.Id == "tenant-001" ? 17 : 9,
                TotalQueries = t.Id == "tenant-001" ? 12 : 5,
                LastActivityDate = DateTime.Now.AddMinutes(-45)
            }).ToList();
        }

        private List<OzBiChat> GetDemoChats()
        {
            var user = new OzBiUser { Id = "usr-01", NameSurname = "Ahmet Yılmaz", Email = "ahmet@testbilisim.com" };
            var tenant1 = GetDemoTenants()[0];

            return new List<OzBiChat>
            {
                new OzBiChat { Id = "chat-101", Title = "Satış Analizi ve Ödeme Almayan Cariler", TenantId = "tenant-001", Tenant = tenant1, CreatedByUserId = "usr-01", CreatedByUser = user, IsActive = true, DateCreated = DateTime.Now.AddHours(-3) },
                new OzBiChat { Id = "chat-102", Title = "En Yüksek Adet Satılan Ürünler İlk 10", TenantId = "tenant-001", Tenant = tenant1, CreatedByUserId = "usr-01", CreatedByUser = user, IsActive = true, DateCreated = DateTime.Now.AddHours(-5) },
                new OzBiChat { Id = "chat-103", Title = "Aylara Göre Toplam Satış ve Ödenmiş Faturalar", TenantId = "tenant-001", Tenant = tenant1, CreatedByUserId = "usr-01", CreatedByUser = user, IsActive = true, DateCreated = DateTime.Now.AddDays(-1) }
            };
        }

        private List<ChatAuditSummary> GetDemoChatsSummary(string tenantId, string? searchTerm)
        {
            var chats = GetDemoChats().Where(c => c.TenantId == tenantId).ToList();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                chats = chats.Where(c => c.Title != null && c.Title.ToLower().Contains(term)).ToList();
            }

            return chats.Select(c => new ChatAuditSummary
            {
                Chat = c,
                MessageCount = c.Id == "chat-101" ? 5 : 3,
                QueryCount = c.Id == "chat-101" ? 4 : 2,
                TotalDurationMs = 1840,
                LastMessageDate = c.DateCreated,
                PrimaryAiModelName = "GPT-4o (OpenAI)"
            }).ToList();
        }

        private List<OzBiChatMessage> GetDemoMessages(string chatId)
        {
            var gpt4 = new OzBiAiModel { Id = "m-1", Name = "GPT-4o (OpenAI)", ProgrammaticName = "gpt-4o", TokenLimit = 128000, HasTools = true, HasMcpSupport = true };
            var claude = new OzBiAiModel { Id = "m-2", Name = "Claude 3.5 Sonnet (Anthropic)", ProgrammaticName = "claude-3-5-sonnet", TokenLimit = 200000, HasTools = true };
            var asst = new OzBiAssistant { Id = "a-1", Name = "Mikro ERP Satış Danışmanı", Description = "Logo & Mikro ERP verilerini analiz eder" };

            return new List<OzBiChatMessage>
            {
                new OzBiChatMessage
                {
                    Id = "msg-01",
                    ChatId = chatId,
                    Role = "user",
                    Prompt = "Son 90 günde ödeme yapmayan cari hesapların listesini ve bakiyelerini tavsiye vererek getir",
                    DateCreated = DateTime.Now.AddHours(-3).AddMinutes(1)
                },
                new OzBiChatMessage
                {
                    Id = "msg-02",
                    ChatId = chatId,
                    Role = "assistant",
                    AIModelId = "m-1",
                    AIModel = gpt4,
                    Assistant = asst,
                    IsSucceeded = true,
                    Prompt = "Son 90 günde ödeme yapmayan cariler ve bakiyeleri",
                    Message = "Son 90 günde ödeme yapmayan carilerinize ait SQL sorgusu oluşturuldu ve veriler listelendi. Toplam 14 cari hesabın 450.000 TL gecikmiş borcu bulunmaktadır. Vadesi 60 günü geçen müşteriler için risk uyarısı yapılması tavsiye olunur.",
                    Query = @"SELECT 
    cha_kod AS [Cari Kod],
    cha_unvan AS [Cari Unvan],
    SUM(CASE WHEN cha_evrak_tip IN (1, 3) THEN cha_meblag ELSE -cha_meblag END) AS [Toplam Borç Bakiye],
    MAX(cha_tarih) AS [Son İşlem Tarihi]
FROM CARI_HESAP_HAREKETLERI WITH (NOLOCK)
WHERE cha_tarih >= DATEADD(day, -90, GETDATE())
GROUP BY cha_kod, cha_unvan
HAVING SUM(CASE WHEN cha_evrak_tip IN (1, 3) THEN cha_meblag ELSE -cha_meblag END) > 0
ORDER BY [Toplam Borç Bakiye] DESC;",
                    TotalDurationMs = 1250,
                    AIQueryDurationMs = 820,
                    DataFetchDurationMs = 430,
                    Limit = 50,
                    HasComplicatedQuery = true,
                    DateCreated = DateTime.Now.AddHours(-3).AddMinutes(2)
                },
                new OzBiChatMessage
                {
                    Id = "msg-03",
                    ChatId = chatId,
                    Role = "assistant",
                    AIModelId = "m-2",
                    AIModel = claude,
                    Assistant = asst,
                    IsSucceeded = false,
                    Prompt = "Marmara bölgesinde en çok satan bayileri getir",
                    ErrorMessage = "Invalid column name 'cha_bolge_kod' in WHERE clause.",
                    Query = @"SELECT TOP 10 cha_kod, cha_unvan, SUM(sth_tutar) AS Ciro FROM CARI_HESAP_HAREKETLERI JOIN STOK_HAREKETLERI ON cha_kod = sth_cari_kodu WHERE cha_bolge_kod = 'MARMARA' GROUP BY cha_kod, cha_unvan ORDER BY Ciro DESC;",
                    TotalDurationMs = 610,
                    AIQueryDurationMs = 550,
                    DataFetchDurationMs = 60,
                    DateCreated = DateTime.Now.AddHours(-3).AddMinutes(5)
                }
            };
        }

        private List<OzBiChatMessage> GetDemoGlobalQueries(string? querySearch, bool? failedOnly, long? minDurationMs)
        {
            var msgs = GetDemoMessages("chat-101");
            var chat = GetDemoChats()[0];
            foreach (var m in msgs)
            {
                m.Chat = chat;
            }

            var queries = msgs.Where(m => !string.IsNullOrEmpty(m.Query)).ToList();

            if (!string.IsNullOrWhiteSpace(querySearch))
            {
                var term = querySearch.ToLower();
                queries = queries.Where(q => q.Query!.ToLower().Contains(term) || (q.Prompt != null && q.Prompt.ToLower().Contains(term))).ToList();
            }

            if (failedOnly == true)
            {
                queries = queries.Where(q => !q.IsSucceeded || !string.IsNullOrEmpty(q.ErrorMessage)).ToList();
            }

            if (minDurationMs.HasValue && minDurationMs.Value > 0)
            {
                queries = queries.Where(q => q.TotalDurationMs >= minDurationMs.Value).ToList();
            }

            return queries;
        }
        #endregion
    }
}
