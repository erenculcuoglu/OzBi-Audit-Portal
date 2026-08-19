using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OzBiPortalCRM.Data;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public class OzBiFeedbackMonitorService : BackgroundService, IOzBiFeedbackMonitorService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OzBiFeedbackMonitorService> _logger;

        public OzBiFeedbackMonitorService(
            IServiceProvider serviceProvider,
            ILogger<OzBiFeedbackMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OzBI Müşteri Geri Bildirimi & Beğenilmeyen SQL Takip Servisi (Feedback Monitor) başlatıldı.");

            // Uygulama açılışında DB bağlantısının oturmasını bekle
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndPushNewFeedbacksAsync(pushAllUnpushed: false, triggeredBy: "AutoDaemon");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OzBI feedback canlı takibi sırasında hata oluştu.");
                }

                // 20 saniyede bir MariaDB'yi kontrol et
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        public async Task<int> CheckAndPushNewFeedbacksAsync(bool pushAllUnpushed = false, string? triggeredBy = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var ozBiDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OzBiDbContext>>();
            var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            var slackService = scope.ServiceProvider.GetRequiredService<ISlackNotificationService>();

            using var appDb = await appDbFactory.CreateDbContextAsync();
            await appDb.EnsureTablesCreatedAsync();

            // 1. SQLite kalıcı deposundaki daha önce push edilmiş mesaj ID'lerini yükle
            Dictionary<string, FeedbackPushSnapshot> pushedMap;
            bool isFirstSystemRun = false;
            try
            {
                var list = await appDb.FeedbackPushSnapshots.ToListAsync();
                pushedMap = list.ToDictionary(x => x.MessageId, x => x, StringComparer.OrdinalIgnoreCase);
                isFirstSystemRun = pushedMap.Count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeedbackPushSnapshots tablosu okunamadı, işlem geçici olarak erteleniyor.");
                return 0;
            }

            // 2. MariaDB'den beğenilmeyen veya yorum içeren mesajları oku
            using var ozBiDb = await ozBiDbFactory.CreateDbContextAsync();
            var feedbackMessages = await ozBiDb.ChatMessages.AsNoTracking()
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.Tenant)
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.CreatedByUser)
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                .Where(m => m.IsLiked == false || (!string.IsNullOrEmpty(m.FeedbackReason) && m.FeedbackReason.Trim() != ""))
                .OrderByDescending(m => m.DateCreated)
                .Take(100)
                .ToListAsync();

            if (feedbackMessages.Count == 0) return 0;

            // İlk sistem açılışında geçmiş 100 kaydı birden Slack'e basıp spam yapmamak için,
            // pushAllUnpushed false ise ilk çalıştırmada mevcut eski kayıtları snapshot'a kaydet
            if (isFirstSystemRun && !pushAllUnpushed)
            {
                _logger.LogInformation("İlk çalıştırma: Mevcut {Count} adet geçmiş geri bildirim snapshot'a kaydediliyor (Slack spam koruması).", feedbackMessages.Count);
                foreach (var msg in feedbackMessages)
                {
                    appDb.FeedbackPushSnapshots.Add(new FeedbackPushSnapshot
                    {
                        MessageId = msg.Id,
                        ChatId = msg.ChatId,
                        TenantName = msg.Chat?.Tenant?.Name ?? "Tenant",
                        UserName = msg.Chat?.CreatedByUser?.NameSurname ?? msg.Chat?.CreatedByUser?.Email ?? "Kullanıcı",
                        UserEmail = msg.Chat?.CreatedByUser?.Email,
                        FeedbackReason = msg.FeedbackReason,
                        IsLiked = msg.IsLiked,
                        PushedAt = DateTime.UtcNow,
                        PushedBy = "InitialSnapshot",
                        Status = "InitialSeeded"
                    });
                }
                await appDb.SaveChangesAsync();
                return 0;
            }

            // Prompt eksikse chat geçmişinden kullanıcı sorusunu çek
            var missingPromptChatIds = feedbackMessages
                .Where(m => string.IsNullOrWhiteSpace(m.Prompt) && !string.IsNullOrEmpty(m.ChatId))
                .Select(m => m.ChatId)
                .Distinct()
                .ToList();

            var userPromptsMap = new Dictionary<string, string>();
            if (missingPromptChatIds.Count > 0)
            {
                var userMessages = await ozBiDb.ChatMessages.AsNoTracking()
                    .Where(m => missingPromptChatIds.Contains(m.ChatId) && (m.Role == "user" || m.Role == "User"))
                    .Select(m => new { m.ChatId, m.Message, m.Prompt, m.DateCreated })
                    .ToListAsync();

                foreach (var u in userMessages.OrderByDescending(x => x.DateCreated))
                {
                    if (!userPromptsMap.ContainsKey(u.ChatId))
                    {
                        userPromptsMap[u.ChatId] = !string.IsNullOrWhiteSpace(u.Message) ? u.Message : (u.Prompt ?? string.Empty);
                    }
                }
            }

            int pushedCount = 0;

            foreach (var msg in feedbackMessages)
            {
                if (pushedMap.ContainsKey(msg.Id) && !pushAllUnpushed)
                {
                    continue;
                }

                var promptText = !string.IsNullOrWhiteSpace(msg.Prompt) 
                    ? msg.Prompt 
                    : (userPromptsMap.TryGetValue(msg.ChatId, out var up) ? up : msg.Chat?.Title);

                var formattedSql = ExtractFormattedSql(msg.Query);
                var tenantName = msg.Chat?.Tenant?.Name ?? "Tenant";
                var userName = msg.Chat?.CreatedByUser?.NameSurname ?? msg.Chat?.CreatedByUser?.UserName ?? "Kullanıcı";
                var userEmail = msg.Chat?.CreatedByUser?.Email ?? "E-posta yok";

                var payload = new CustomerFeedbackSlackPayload
                {
                    MessageId = msg.Id,
                    ChatId = msg.ChatId,
                    TenantName = tenantName,
                    UserName = userName,
                    UserEmail = userEmail,
                    AIModelName = msg.AIModel?.Name ?? msg.AIModel?.ProgrammaticName,
                    AssistantName = msg.Assistant?.Name,
                    DateCreated = msg.DateCreated,
                    IsLiked = msg.IsLiked,
                    FeedbackReason = msg.FeedbackReason,
                    Prompt = promptText,
                    GeneratedSql = formattedSql,
                    AIResponse = !string.IsNullOrWhiteSpace(msg.Message) ? msg.Message : msg.Summary,
                    ErrorMessage = msg.ErrorMessage,
                    DurationMs = msg.TotalDurationMs,
                    PushedBy = triggeredBy ?? "AutoDaemon"
                };

                bool sent = await slackService.SendCustomerFeedbackNotificationAsync(payload);

                if (pushedMap.TryGetValue(msg.Id, out var existing))
                {
                    existing.PushedAt = DateTime.UtcNow;
                    existing.PushedBy = triggeredBy ?? "ManualRePush";
                    existing.Status = sent ? "Success" : "Failed";
                }
                else
                {
                    var newSnap = new FeedbackPushSnapshot
                    {
                        MessageId = msg.Id,
                        ChatId = msg.ChatId,
                        TenantName = tenantName,
                        UserName = userName,
                        UserEmail = userEmail,
                        FeedbackReason = msg.FeedbackReason,
                        IsLiked = msg.IsLiked,
                        PushedAt = DateTime.UtcNow,
                        PushedBy = triggeredBy ?? "AutoDaemon",
                        Status = sent ? "Success" : "Failed"
                    };
                    appDb.FeedbackPushSnapshots.Add(newSnap);
                    pushedMap[msg.Id] = newSnap;
                }

                if (sent) pushedCount++;
            }

            if (pushedCount > 0 || appDb.ChangeTracker.HasChanges())
            {
                await appDb.SaveChangesAsync();
            }

            return pushedCount;
        }

        public async Task<bool> PushFeedbackByIdAsync(string messageId, string? pushedBy = null)
        {
            if (string.IsNullOrWhiteSpace(messageId)) return false;

            using var scope = _serviceProvider.CreateScope();
            var ozBiDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OzBiDbContext>>();
            var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            var slackService = scope.ServiceProvider.GetRequiredService<ISlackNotificationService>();

            using var ozBiDb = await ozBiDbFactory.CreateDbContextAsync();
            var msg = await ozBiDb.ChatMessages.AsNoTracking()
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.Tenant)
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.CreatedByUser)
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (msg == null) return false;

            var promptText = msg.Prompt;
            if (string.IsNullOrWhiteSpace(promptText) && !string.IsNullOrEmpty(msg.ChatId))
            {
                var userMsg = await ozBiDb.ChatMessages.AsNoTracking()
                    .Where(m => m.ChatId == msg.ChatId && (m.Role == "user" || m.Role == "User") && m.DateCreated <= msg.DateCreated)
                    .OrderByDescending(m => m.DateCreated)
                    .FirstOrDefaultAsync();

                if (userMsg != null)
                {
                    promptText = !string.IsNullOrWhiteSpace(userMsg.Message) ? userMsg.Message : userMsg.Prompt;
                }
            }

            var formattedSql = ExtractFormattedSql(msg.Query);
            var tenantName = msg.Chat?.Tenant?.Name ?? "Tenant";
            var userName = msg.Chat?.CreatedByUser?.NameSurname ?? msg.Chat?.CreatedByUser?.UserName ?? "Kullanıcı";
            var userEmail = msg.Chat?.CreatedByUser?.Email ?? "E-posta yok";

            var payload = new CustomerFeedbackSlackPayload
            {
                MessageId = msg.Id,
                ChatId = msg.ChatId,
                TenantName = tenantName,
                UserName = userName,
                UserEmail = userEmail,
                AIModelName = msg.AIModel?.Name ?? msg.AIModel?.ProgrammaticName,
                AssistantName = msg.Assistant?.Name,
                DateCreated = msg.DateCreated,
                IsLiked = msg.IsLiked,
                FeedbackReason = msg.FeedbackReason,
                Prompt = !string.IsNullOrWhiteSpace(promptText) ? promptText : msg.Chat?.Title,
                GeneratedSql = formattedSql,
                AIResponse = !string.IsNullOrWhiteSpace(msg.Message) ? msg.Message : msg.Summary,
                ErrorMessage = msg.ErrorMessage,
                DurationMs = msg.TotalDurationMs,
                PushedBy = pushedBy ?? "ManualCRM"
            };

            bool sent = await slackService.SendCustomerFeedbackNotificationAsync(payload);

            using var appDb = await appDbFactory.CreateDbContextAsync();
            await appDb.EnsureTablesCreatedAsync();

            var existingSnap = await appDb.FeedbackPushSnapshots.FirstOrDefaultAsync(s => s.MessageId == messageId);
            if (existingSnap != null)
            {
                existingSnap.PushedAt = DateTime.UtcNow;
                existingSnap.PushedBy = pushedBy ?? "ManualCRM";
                existingSnap.Status = sent ? "Success" : "Failed";
                existingSnap.FeedbackReason = msg.FeedbackReason;
                existingSnap.IsLiked = msg.IsLiked;
                appDb.FeedbackPushSnapshots.Update(existingSnap);
            }
            else
            {
                appDb.FeedbackPushSnapshots.Add(new FeedbackPushSnapshot
                {
                    MessageId = msg.Id,
                    ChatId = msg.ChatId,
                    TenantName = tenantName,
                    UserName = userName,
                    UserEmail = userEmail,
                    FeedbackReason = msg.FeedbackReason,
                    IsLiked = msg.IsLiked,
                    PushedAt = DateTime.UtcNow,
                    PushedBy = pushedBy ?? "ManualCRM",
                    Status = sent ? "Success" : "Failed"
                });
            }

            await appDb.SaveChangesAsync();
            return sent;
        }

        public async Task<HashSet<string>> GetPushedMessageIdsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var appDb = await appDbFactory.CreateDbContextAsync();
                await appDb.EnsureTablesCreatedAsync();

                var ids = await appDb.FeedbackPushSnapshots
                    .Where(s => s.Status == "Success")
                    .Select(s => s.MessageId)
                    .ToListAsync();

                return ids.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pushed message IDs okunamadı.");
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public async Task<List<FeedbackPushSnapshot>> GetPushedSnapshotsAsync(int limit = 100)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var appDb = await appDbFactory.CreateDbContextAsync();
                await appDb.EnsureTablesCreatedAsync();

                return await appDb.FeedbackPushSnapshots.AsNoTracking()
                    .OrderByDescending(s => s.PushedAt)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pushed snapshots okunamadı.");
                return new List<FeedbackPushSnapshot>();
            }
        }

        private static string ExtractFormattedSql(string? rawQuery)
        {
            if (string.IsNullOrWhiteSpace(rawQuery)) return string.Empty;

            var trimmed = rawQuery.Trim();
            if ((trimmed.StartsWith("[") && trimmed.EndsWith("]")) || (trimmed.StartsWith("{") && trimmed.EndsWith("}")))
            {
                try
                {
                    using var doc = JsonDocument.Parse(trimmed);
                    var sqlList = new List<string>();

                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var elem in doc.RootElement.EnumerateArray())
                        {
                            var s = ExtractSingleSql(elem);
                            if (!string.IsNullOrWhiteSpace(s)) sqlList.Add(s);
                        }
                    }
                    else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        var s = ExtractSingleSql(doc.RootElement);
                        if (!string.IsNullOrWhiteSpace(s)) sqlList.Add(s);
                    }

                    if (sqlList.Count > 0)
                    {
                        return string.Join("\n\n-- ----------------------------\n\n", sqlList);
                    }
                }
                catch
                {
                    // Fallback to raw string
                }
            }

            return trimmed;
        }

        private static string? ExtractSingleSql(JsonElement elem)
        {
            string desc = string.Empty;
            string sql = string.Empty;

            if (elem.ValueKind == JsonValueKind.Object)
            {
                if (elem.TryGetProperty("description", out var dProp) || elem.TryGetProperty("Description", out dProp) || elem.TryGetProperty("summary", out dProp))
                {
                    desc = dProp.GetString() ?? string.Empty;
                }

                if (elem.TryGetProperty("sql", out var sProp) || elem.TryGetProperty("Sql", out sProp) || elem.TryGetProperty("query", out sProp) || elem.TryGetProperty("Query", out sProp))
                {
                    sql = sProp.GetString() ?? string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                if (elem.ValueKind == JsonValueKind.String) sql = elem.GetString() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(sql)) return null;

            if (!string.IsNullOrWhiteSpace(desc))
            {
                return $"-- {desc}\n{sql}";
            }

            return sql;
        }
    }
}
