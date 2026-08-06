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

                var webhookUrl = _configuration["Slack:WebhookUrl"];
                if (string.IsNullOrWhiteSpace(webhookUrl) || webhookUrl.Contains("YOUR/SLACK/WEBHOOK_URL"))
                {
                    webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
                }

                if (string.IsNullOrWhiteSpace(webhookUrl) || webhookUrl.Contains("YOUR/SLACK/WEBHOOK_URL"))
                {
                    _logger.LogWarning("Slack Webhook URL yapılandırılmamış. appsettings.Development.json veya SLACK_WEBHOOK_URL ortam değişkenini kontrol edin.");
                    return;
                }

                // Türkiye saati (TSI - GMT+3) hesaplama
                DateTime localTime;
                try
                {
                    var turkeyZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                    localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyZone);
                }
                catch
                {
                    try
                    {
                        var europeIstanbul = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, europeIstanbul);
                    }
                    catch
                    {
                        localTime = DateTime.UtcNow.AddHours(3);
                    }
                }

                var formattedTime = localTime.ToString("dd.MM.yyyy HH:mm:ss");

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
    }
}
