using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OzBiLoginMonitorService : BackgroundService, IOzBiLoginMonitorService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OzBiLoginMonitorService> _logger;

        public OzBiLoginMonitorService(
            IServiceProvider serviceProvider,
            ILogger<OzBiLoginMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OzBI MariaDB Kullanıcı Girişi Takip Servisi (Login Monitor - Persistent SQLite) başlatıldı.");

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

        public async Task CheckForNewLoginsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var ozBiDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OzBiDbContext>>();
            var appDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            var slackService = scope.ServiceProvider.GetRequiredService<ISlackNotificationService>();

            using var appDb = await appDbFactory.CreateDbContextAsync();
            await appDb.EnsureTablesCreatedAsync();

            // SQLite kalıcı veritabanındaki takip kayıtlarını EF Core Change Tracker ile yükle
            Dictionary<string, UserLoginSnapshot> savedSnapshots;
            try
            {
                var snapshotList = await appDb.UserLoginSnapshots.ToListAsync();
                savedSnapshots = snapshotList.ToDictionary(s => s.UserId, s => s);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "UserLoginSnapshots tablosu henüz okunamadı.");
                savedSnapshots = new Dictionary<string, UserLoginSnapshot>();
            }

            using var ozBiDb = await ozBiDbFactory.CreateDbContextAsync();
            var users = await ozBiDb.Users.AsNoTracking()
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

            var tenantIds = users.Select(u => u.TenantId).Where(tid => !string.IsNullOrEmpty(tid)).Distinct().ToList();
            var tenantMap = await ozBiDb.Tenants.AsNoTracking()
                .Where(t => tenantIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.Name);

            bool isFirstSystemRun = savedSnapshots.Count == 0;

            foreach (var user in users)
            {
                if (savedSnapshots.TryGetValue(user.Id, out var snapshot))
                {
                    if (user.LoginCount > snapshot.LastSeenLoginCount)
                    {
                        tenantMap.TryGetValue(user.TenantId, out var tenantName);
                        tenantName ??= user.TenantId;

                        string displayName = !string.IsNullOrWhiteSpace(user.NameSurname) ? user.NameSurname : (user.UserName ?? user.Email ?? "Bilinmeyen Kullanıcı");
                        string emailStr = user.Email ?? user.UserName ?? "E-posta yok";

                        _logger.LogInformation("OzBI Giriş Tetiklendi: Firma={Tenant}, Kullanıcı={User}, Eski LoginCount={PrevCount}, Yeni LoginCount={Count}", tenantName, displayName, snapshot.LastSeenLoginCount, user.LoginCount);

                        // EF Core tarafından takip edilen varlığı doğrudan güncelle
                        snapshot.LastSeenLoginCount = user.LoginCount;
                        snapshot.LastUpdatedAt = DateTime.UtcNow;

                        // Slack push bildirimini tetikle
                        await slackService.SendTenantUserLoginNotificationAsync(tenantName, displayName, emailStr, user.LoginCount);
                    }
                }
                else
                {
                    // SQLite veritabanında henüz bulunmayan yeni kullanıcı
                    var newSnapshot = new UserLoginSnapshot
                    {
                        UserId = user.Id,
                        LastSeenLoginCount = user.LoginCount,
                        LastUpdatedAt = DateTime.UtcNow
                    };
                    appDb.UserLoginSnapshots.Add(newSnapshot);
                    savedSnapshots[user.Id] = newSnapshot;

                    if (!isFirstSystemRun && user.LoginCount > 0)
                    {
                        tenantMap.TryGetValue(user.TenantId, out var tenantName);
                        tenantName ??= user.TenantId;
                        string displayName = !string.IsNullOrWhiteSpace(user.NameSurname) ? user.NameSurname : (user.UserName ?? user.Email ?? "Bilinmeyen Kullanıcı");
                        string emailStr = user.Email ?? user.UserName ?? "E-posta yok";

                        await slackService.SendTenantUserLoginNotificationAsync(tenantName, displayName, emailStr, user.LoginCount);
                    }
                }
            }

            await appDb.SaveChangesAsync();
        }
    }
}
