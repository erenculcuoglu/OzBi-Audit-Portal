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

            // SQLite veritabanının ve tabloların oluşturulduğundan emin ol
            using var appDb = await appDbFactory.CreateDbContextAsync();
            await appDb.EnsureTablesCreatedAsync();

            Dictionary<string, int> savedSnapshots;
            try
            {
                savedSnapshots = await appDb.UserLoginSnapshots.AsNoTracking().ToDictionaryAsync(s => s.UserId, s => s.LastSeenLoginCount);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "UserLoginSnapshots tablosu henüz okunamadı, yeni oluşturuluyor.");
                savedSnapshots = new Dictionary<string, int>();
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
                if (savedSnapshots.TryGetValue(user.Id, out var previousCount))
                {
                    if (user.LoginCount > previousCount)
                    {
                        // SQLite kalıcı veritabanında son görülen sayıyı güncelle
                        var existingSnapshot = await appDb.UserLoginSnapshots.FindAsync(user.Id);
                        if (existingSnapshot != null)
                        {
                            existingSnapshot.LastSeenLoginCount = user.LoginCount;
                            existingSnapshot.LastUpdatedAt = DateTime.UtcNow;
                            appDb.UserLoginSnapshots.Update(existingSnapshot);
                        }
                        else
                        {
                            appDb.UserLoginSnapshots.Add(new UserLoginSnapshot
                            {
                                UserId = user.Id,
                                LastSeenLoginCount = user.LoginCount,
                                LastUpdatedAt = DateTime.UtcNow
                            });
                        }

                        tenantMap.TryGetValue(user.TenantId, out var tenantName);
                        tenantName ??= user.TenantId;

                        string displayName = !string.IsNullOrWhiteSpace(user.NameSurname) ? user.NameSurname : (user.UserName ?? user.Email ?? "Bilinmeyen Kullanıcı");
                        string emailStr = user.Email ?? user.UserName ?? "E-posta yok";

                        _logger.LogInformation("OzBI Giriş Tetiklendi: Firma={Tenant}, Kullanıcı={User}, Eski LoginCount={PrevCount}, Yeni LoginCount={Count}", tenantName, displayName, previousCount, user.LoginCount);

                        // Slack push bildirimini tetikle
                        await slackService.SendTenantUserLoginNotificationAsync(tenantName, displayName, emailStr, user.LoginCount);
                    }
                }
                else
                {
                    // İlk defa görülen kullanıcı - SQLite'a kaydet
                    appDb.UserLoginSnapshots.Add(new UserLoginSnapshot
                    {
                        UserId = user.Id,
                        LastSeenLoginCount = user.LoginCount,
                        LastUpdatedAt = DateTime.UtcNow
                    });

                    // Eğer sistemde daha önce kayıtlı hiç snapshot yoksa (ilk kurulum), mevcut kullanıcılar için bildirim gönderme, sadece kaydet
                    // Ancak sistem çalışıyorken yeni bir kullanıcı eklendiyse ve LoginCount > 0 ise bildirim gönder
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
