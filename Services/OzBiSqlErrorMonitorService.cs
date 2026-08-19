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
    public class OzBiSqlErrorMonitorService : BackgroundService, IOzBiSqlErrorMonitorService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OzBiSqlErrorMonitorService> _logger;

        public OzBiSqlErrorMonitorService(
            IServiceProvider serviceProvider,
            ILogger<OzBiSqlErrorMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OzBI SQL / Sistem Hataları Takip Servisi (SQL Error Monitor) başlatıldı.");

            // Uygulama açılışında DB bağlantısının oturmasını bekle
            await Task.Delay(TimeSpan.FromSeconds(7), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndPushNewSqlErrorsAsync(pushAllUnpushed: false, triggeredBy: "AutoDaemon");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OzBI SQL hata canlı takibi sırasında hata oluştu.");
                }

                // 20 saniyede bir MariaDB'yi kontrol et
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        public async Task<int> CheckAndPushNewSqlErrorsAsync(bool pushAllUnpushed = false, string? triggeredBy = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var ozBiDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OzBiDbContext>>();
            var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            var slackService = scope.ServiceProvider.GetRequiredService<ISlackNotificationService>();

            using var appDb = await appDbFactory.CreateDbContextAsync();
            await appDb.EnsureTablesCreatedAsync();

            // 1. SQLite kalıcı deposundaki daha önce bildirilmiş hata ID'lerini yükle
            Dictionary<string, SqlErrorPushSnapshot> pushedMap;
            bool isFirstSystemRun = false;
            try
            {
                var list = await appDb.SqlErrorPushSnapshots.ToListAsync();
                pushedMap = list.ToDictionary(x => x.MessageId, x => x, StringComparer.OrdinalIgnoreCase);
                isFirstSystemRun = pushedMap.Count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SqlErrorPushSnapshots tablosu okunamadı, işlem geçici olarak erteleniyor.");
                return 0;
            }

            // 2. MariaDB'den hata içeren model mesajlarını oku
            using var ozBiDb = await ozBiDbFactory.CreateDbContextAsync();
            var errorMessages = await ozBiDb.ChatMessages.AsNoTracking()
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.Tenant)
                .Include(m => m.Chat)
                    .ThenInclude(c => c!.CreatedByUser)
                .Include(m => m.AIModel)
                .Include(m => m.Assistant)
                .Where(m => (m.ErrorMessage != null && m.ErrorMessage.Trim() != "") 
                         && (m.Role == "Model" || m.Role == "model" || m.Role == "assistant"))
                .OrderByDescending(m => m.DateCreated)
                .Take(100)
                .ToListAsync();

            if (errorMessages.Count == 0) return 0;

            // İlk sistem açılışında geçmiş 100 kaydı birden Slack'e basıp spam yapmamak için,
            // pushAllUnpushed false ise ilk çalıştırmada mevcut eski kayıtları snapshot'a kaydet
            if (isFirstSystemRun && !pushAllUnpushed)
            {
                _logger.LogInformation("İlk çalıştırma: Mevcut {Count} adet geçmiş SQL hatası snapshot'a kaydediliyor (Slack spam koruması).", errorMessages.Count);
                foreach (var msg in errorMessages)
                {
                    appDb.SqlErrorPushSnapshots.Add(new SqlErrorPushSnapshot
                    {
                        MessageId = msg.Id,
                        ChatId = msg.ChatId,
                        TenantName = msg.Chat?.Tenant?.Name ?? "Tenant",
                        UserName = msg.Chat?.CreatedByUser?.NameSurname ?? msg.Chat?.CreatedByUser?.Email ?? "Kullanıcı",
                        UserEmail = msg.Chat?.CreatedByUser?.Email,
                        ErrorMessage = msg.ErrorMessage,
                        Prompt = msg.Prompt ?? msg.Chat?.Title,
                        SqlQuery = ExtractFormattedSql(msg.Query),
                        PushedAt = DateTime.UtcNow,
                        PushedBy = "InitialSnapshot",
                        Status = "InitialSeeded"
                    });
                }
                await appDb.SaveChangesAsync();
                return 0;
            }

            // Prompt eksikse chat geçmişinden kullanıcı sorusunu çek
            var missingPromptChatIds = errorMessages
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

            foreach (var msg in errorMessages)
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

                var payload = new SqlErrorSlackPayload
                {
                    MessageId = msg.Id,
                    ChatId = msg.ChatId,
                    TenantName = tenantName,
                    UserName = userName,
                    UserEmail = userEmail,
                    AIModelName = msg.AIModel?.Name ?? msg.AIModel?.ProgrammaticName,
                    AssistantName = msg.Assistant?.Name,
                    DateCreated = msg.DateCreated,
                    ErrorMessage = msg.ErrorMessage,
                    Prompt = promptText,
                    SqlQuery = formattedSql,
                    AIResponse = !string.IsNullOrWhiteSpace(msg.Message) ? msg.Message : msg.Summary,
                    DurationMs = msg.TotalDurationMs,
                    PushedBy = triggeredBy ?? "AutoDaemon"
                };

                bool sent = await slackService.SendSqlErrorNotificationAsync(payload);

                if (pushedMap.TryGetValue(msg.Id, out var existing))
                {
                    existing.PushedAt = DateTime.UtcNow;
                    existing.PushedBy = triggeredBy ?? "ManualRePush";
                    existing.Status = sent ? "Success" : "Failed";
                }
                else
                {
                    var newSnap = new SqlErrorPushSnapshot
                    {
                        MessageId = msg.Id,
                        ChatId = msg.ChatId,
                        TenantName = tenantName,
                        UserName = userName,
                        UserEmail = userEmail,
                        ErrorMessage = msg.ErrorMessage,
                        Prompt = promptText,
                        SqlQuery = formattedSql,
                        PushedAt = DateTime.UtcNow,
                        PushedBy = triggeredBy ?? "AutoDaemon",
                        Status = sent ? "Success" : "Failed"
                    };
                    appDb.SqlErrorPushSnapshots.Add(newSnap);
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

        public async Task<bool> PushSqlErrorByIdAsync(string messageId, string? pushedBy = null)
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

            var payload = new SqlErrorSlackPayload
            {
                MessageId = msg.Id,
                ChatId = msg.ChatId,
                TenantName = tenantName,
                UserName = userName,
                UserEmail = userEmail,
                AIModelName = msg.AIModel?.Name ?? msg.AIModel?.ProgrammaticName,
                AssistantName = msg.Assistant?.Name,
                DateCreated = msg.DateCreated,
                ErrorMessage = msg.ErrorMessage,
                Prompt = promptText,
                SqlQuery = formattedSql,
                AIResponse = !string.IsNullOrWhiteSpace(msg.Message) ? msg.Message : msg.Summary,
                DurationMs = msg.TotalDurationMs,
                PushedBy = pushedBy ?? "ManualPush"
            };

            bool sent = await slackService.SendSqlErrorNotificationAsync(payload);

            using var appDb = await appDbFactory.CreateDbContextAsync();
            await appDb.EnsureTablesCreatedAsync();

            var existing = await appDb.SqlErrorPushSnapshots.FirstOrDefaultAsync(x => x.MessageId == msg.Id);
            if (existing != null)
            {
                existing.PushedAt = DateTime.UtcNow;
                existing.PushedBy = pushedBy ?? "ManualPush";
                existing.Status = sent ? "Success" : "Failed";
            }
            else
            {
                appDb.SqlErrorPushSnapshots.Add(new SqlErrorPushSnapshot
                {
                    MessageId = msg.Id,
                    ChatId = msg.ChatId,
                    TenantName = tenantName,
                    UserName = userName,
                    UserEmail = userEmail,
                    ErrorMessage = msg.ErrorMessage,
                    Prompt = promptText,
                    SqlQuery = formattedSql,
                    PushedAt = DateTime.UtcNow,
                    PushedBy = pushedBy ?? "ManualPush",
                    Status = sent ? "Success" : "Failed"
                });
            }

            await appDb.SaveChangesAsync();
            return sent;
        }

        public async Task<HashSet<string>> GetPushedSqlErrorMessageIdsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var appDb = await appDbFactory.CreateDbContextAsync();
                await appDb.EnsureTablesCreatedAsync();

                var list = await appDb.SqlErrorPushSnapshots
                    .Select(x => x.MessageId)
                    .ToListAsync();

                return new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPushedSqlErrorMessageIdsAsync hatası.");
                return new HashSet<string>();
            }
        }

        public async Task<List<SqlErrorPushSnapshot>> GetPushedSnapshotsAsync(int limit = 100)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var appDb = await appDbFactory.CreateDbContextAsync();
                await appDb.EnsureTablesCreatedAsync();

                return await appDb.SqlErrorPushSnapshots
                    .OrderByDescending(x => x.PushedAt)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPushedSnapshotsAsync hatası.");
                return new List<SqlErrorPushSnapshot>();
            }
        }

        private static string? ExtractFormattedSql(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            raw = raw.Trim();
            if (raw.StartsWith("[") || raw.StartsWith("{"))
            {
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var sqls = new List<string>();

                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var el in doc.RootElement.EnumerateArray())
                        {
                            var desc = el.TryGetProperty("description", out var d) ? d.GetString() :
                                       (el.TryGetProperty("Description", out var d2) ? d2.GetString() :
                                       (el.TryGetProperty("summary", out var s) ? s.GetString() : null));

                            var sql = el.TryGetProperty("sql", out var q) ? q.GetString() :
                                      (el.TryGetProperty("Sql", out var q2) ? q2.GetString() :
                                      (el.TryGetProperty("query", out var q3) ? q3.GetString() :
                                      (el.TryGetProperty("Query", out var q4) ? q4.GetString() :
                                      (el.TryGetProperty("result", out var q5) ? q5.GetString() : null))));

                            if (!string.IsNullOrWhiteSpace(sql))
                            {
                                sqls.Add(!string.IsNullOrWhiteSpace(desc) ? $"-- {desc}\n{sql}" : sql);
                            }
                        }
                    }
                    else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        var root = doc.RootElement;
                        var desc = root.TryGetProperty("description", out var d) ? d.GetString() :
                                   (root.TryGetProperty("Description", out var d2) ? d2.GetString() :
                                   (root.TryGetProperty("summary", out var s) ? s.GetString() : null));

                        var sql = root.TryGetProperty("sql", out var q) ? q.GetString() :
                                  (root.TryGetProperty("Sql", out var q2) ? q2.GetString() :
                                  (root.TryGetProperty("query", out var q3) ? q3.GetString() :
                                  (root.TryGetProperty("Query", out var q4) ? q4.GetString() :
                                  (root.TryGetProperty("result", out var q5) ? q5.GetString() : null))));

                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            sqls.Add(!string.IsNullOrWhiteSpace(desc) ? $"-- {desc}\n{sql}" : sql);
                        }
                    }

                    if (sqls.Count > 0)
                    {
                        return string.Join("\n\n-- ----------------------------\n\n", sqls);
                    }
                }
                catch
                {
                    // Fallback to raw
                }
            }

            return raw;
        }
    }
}
