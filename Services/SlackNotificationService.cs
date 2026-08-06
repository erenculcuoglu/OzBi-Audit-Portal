using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OzBiPortalCRM.Services
{
    public class SlackNotificationService : ISlackNotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SlackNotificationService> _logger;

        public SlackNotificationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SlackNotificationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        private string GetEffectiveWebhookUrl()
        {
            // 1. appsettings.json / appsettings.Production.json (deploy sırasında enjekte edilir)
            var webhookUrl = _configuration["Slack:WebhookUrl"];

            // 2. Ortam değişkeni fallback
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
            }

            return webhookUrl ?? string.Empty;
        }

        private static DateTime GetTurkeyTime()
        {
            try
            {
                var turkeyZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyZone);
            }
            catch
            {
                try
                {
                    var europeIstanbul = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, europeIstanbul);
                }
                catch
                {
                    return DateTime.UtcNow.AddHours(3);
                }
            }
        }

        public async Task SendLoginNotificationAsync(string fullName, string email, string role, string ipAddress, string? userAgent = null)
        {
            try
            {
                var enabled = _configuration.GetValue<bool>("Slack:Enabled", true);
                if (!enabled)
                {
                    _logger.LogInformation("Slack bildirimleri yapılandırmada kapalı tutuluyor.");
                    return;
                }

                var webhookUrl = GetEffectiveWebhookUrl();
                if (string.IsNullOrWhiteSpace(webhookUrl))
                {
                    _logger.LogWarning("Slack Webhook URL yapılandırılmamış.");
                    return;
                }

                var formattedTime = GetTurkeyTime().ToString("dd.MM.yyyy HH:mm:ss");

                var payload = new
                {
                    text = $"🔐 *OzBI Portal Login:* {fullName} ({email}) giriş yaptı.",
                    blocks = new object[]
                    {
                        new
                        {
                            type = "header",
                            text = new
                            {
                                type = "plain_text",
                                text = "🔐 OzBI Portal - Yeni Kullanıcı Girişi",
                                emoji = true
                            }
                        },
                        new
                        {
                            type = "section",
                            fields = new object[]
                            {
                                new { type = "mrkdwn", text = $"*Kullanıcı Adı:*\n{fullName}" },
                                new { type = "mrkdwn", text = $"*E-Posta:*\n`{email}`" },
                                new { type = "mrkdwn", text = $"*Yetki / Rol:*\n{role}" },
                                new { type = "mrkdwn", text = $"*Giriş Zamanı:*\n{formattedTime} (TSI)" },
                                new { type = "mrkdwn", text = $"*IP Adresi:*\n`{ipAddress}`" }
                            }
                        },
                        new
                        {
                            type = "context",
                            elements = new object[]
                            {
                                new { type = "mrkdwn", text = $"🌐 *Cihaz / Agent:* {(string.IsNullOrWhiteSpace(userAgent) ? "Bilinmiyor" : userAgent)}" }
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var client = _httpClientFactory.CreateClient("SlackClient");

                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Slack login bildirimi başarıyla gönderildi: {Email}", email);
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Slack bildirim gönderimi başarısız oldu. Kod: {StatusCode}, Yanıt: {ResponseBody}", response.StatusCode, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slack bildirimi gönderilirken beklenmeyen bir hata oluştu.");
            }
        }

        public async Task SendTenantUserLoginNotificationAsync(string tenantName, string fullName, string email, int totalLoginCount)
        {
            try
            {
                var enabled = _configuration.GetValue<bool>("Slack:Enabled", true);
                if (!enabled)
                {
                    return;
                }

                var webhookUrl = GetEffectiveWebhookUrl();
                if (string.IsNullOrWhiteSpace(webhookUrl))
                {
                    _logger.LogWarning("Slack Webhook URL yapılandırılmamış.");
                    return;
                }

                var formattedTime = GetTurkeyTime().ToString("dd.MM.yyyy HH:mm:ss");

                var payload = new
                {
                    text = $"🟢 *OzBI App Kullanıcı Girişi:* {tenantName} / {fullName} ({email})",
                    blocks = new object[]
                    {
                        new
                        {
                            type = "header",
                            text = new
                            {
                                type = "plain_text",
                                text = "🚀 OzBI Uygulaması - Kullanıcı Girişi",
                                emoji = true
                            }
                        },
                        new
                        {
                            type = "section",
                            fields = new object[]
                            {
                                new { type = "mrkdwn", text = $"*Firma / Tenant:*\n*{tenantName}*" },
                                new { type = "mrkdwn", text = $"*Kullanıcı:*\n{fullName}" },
                                new { type = "mrkdwn", text = $"*E-Posta:*\n`{email}`" },
                                new { type = "mrkdwn", text = $"*Toplam Giriş Sayısı:*\n`{totalLoginCount}`" },
                                new { type = "mrkdwn", text = $"*Giriş Zamanı:*\n{formattedTime} (TSI)" }
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var client = _httpClientFactory.CreateClient("SlackClient");

                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Slack tenant user login bildirimi başarıyla gönderildi: {Tenant} / {Email}", tenantName, email);
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Slack tenant bildirim gönderimi başarısız oldu. Kod: {StatusCode}, Yanıt: {ResponseBody}", response.StatusCode, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slack tenant bildirimi gönderilirken beklenmeyen bir hata oluştu.");
            }
        }
    }
}
