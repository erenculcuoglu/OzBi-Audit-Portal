using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OzBiPortalCRM.Data;

namespace OzBiPortalCRM.Services
{
    public class OzBiLoginMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OzBiLoginMonitorService> _logger;

        private readonly ConcurrentDictionary<string, int> _userLoginCounts = new();
        private bool _isInitialSnapshotDone = false;

        public OzBiLoginMonitorService(
            IServiceProvider serviceProvider,
            ILogger<OzBiLoginMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OzBI MariaDB Kullanıcı Girişi Takip Servisi (Login Monitor) başlatıldı.");

            // Uygulama açılışında DB bağlantısının oturmasını bekle
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForNewLoginsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OzBI MariaDB kullanıcı login takibi sırasında hata oluştu.");
                }

                // 10 saniyede bir MariaDB'yi kontrol et
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task CheckForNewLoginsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OzBiDbContext>>();
            var slackService = scope.ServiceProvider.GetRequiredService<ISlackNotificationService>();

            using var db = await dbFactory.CreateDbContextAsync();

            var users = await db.Users.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Select(u => new
                {
                    u.Id,
                    u.NameSurname,
                    u.Email,
                    u.UserName,
                    u.TenantId,
                    u.LoginCount
                })
                .ToListAsync();

            if (!_isInitialSnapshotDone)
            {
                foreach (var user in users)
                {
                    _userLoginCounts[user.Id] = user.LoginCount;
                }
                _isInitialSnapshotDone = true;
                _logger.LogInformation("OzBI MariaDB kullanıcı haritası {Count} kullanıcı için ilk durum kaydedildi.", users.Count);
                return;
            }

            var tenantIds = users.Select(u => u.TenantId).Where(tid => !string.IsNullOrEmpty(tid)).Distinct().ToList();
            var tenantMap = await db.Tenants.AsNoTracking()
                .Where(t => tenantIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.Name);

            foreach (var user in users)
            {
                if (_userLoginCounts.TryGetValue(user.Id, out var previousCount))
                {
                    if (user.LoginCount > previousCount)
                    {
                        _userLoginCounts[user.Id] = user.LoginCount;

                        tenantMap.TryGetValue(user.TenantId, out var tenantName);
                        tenantName ??= user.TenantId;

                        string displayName = !string.IsNullOrWhiteSpace(user.NameSurname) ? user.NameSurname : (user.UserName ?? user.Email ?? "Bilinmeyen Kullanıcı");
                        string emailStr = user.Email ?? user.UserName ?? "E-posta yok";

                        _logger.LogInformation("OzBI Giriş Tetiklendi: Firma={Tenant}, Kullanıcı={User}, Yeni LoginCount={Count}", tenantName, displayName, user.LoginCount);

                        // Slack push bildirimini tetikle
                        await slackService.SendTenantUserLoginNotificationAsync(tenantName, displayName, emailStr, user.LoginCount);
                    }
                }
                else
                {
                    _userLoginCounts[user.Id] = user.LoginCount;
                }
            }
        }
    }
}
