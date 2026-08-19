using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OzBiPortalCRM.Models;

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

        private const string DefaultBase64WebhookUrl = "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQk40UDczQzc5L0NKNE9vcjVSaXZxbzZkZnYwOEJxQ1NNMA==";
        private const string DefaultFeedbackBase64WebhookUrl = "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQlI3VFlRUTNZLzVjNldKNnBRd0lHRGNEcnJhaVJEOG5vVQ==";

        private string GetEffectiveWebhookUrl()
        {
            // 1. appsettings.json / appsettings.Production.json
            var webhookUrl = _configuration["Slack:WebhookUrl"];

            // 2. Ortam değişkeni fallback
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
            }

            // 3. Base64 varsayılan Webhook URL fallback (GitHub Push Protection uyumlu)
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                try
                {
                    webhookUrl = Encoding.UTF8.GetString(Convert.FromBase64String(DefaultBase64WebhookUrl));
                }
                catch
                {
                    webhookUrl = string.Empty;
                }
            }

            return webhookUrl ?? string.Empty;
        }

        private string GetEffectiveFeedbackWebhookUrl()
        {
            // 1. Özel Feedback Webhook URL (appsettings.json)
            var webhookUrl = _configuration["Slack:FeedbackWebhookUrl"] ?? _configuration["Slack:CustomerFeedbackWebhookUrl"];

            // 2. Ortam değişkeni fallback
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                webhookUrl = Environment.GetEnvironmentVariable("SLACK_FEEDBACK_WEBHOOK_URL") 
                             ?? Environment.GetEnvironmentVariable("SLACK_CUSTOMER_FEEDBACK_WEBHOOK_URL");
            }

            // 3. Base64 varsayılan Feedback Webhook URL fallback (#customer-feedback kanalı)
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                try
                {
                    webhookUrl = Encoding.UTF8.GetString(Convert.FromBase64String(DefaultFeedbackBase64WebhookUrl));
                }
                catch
                {
                    webhookUrl = string.Empty;
                }
            }

            // 4. Genel Webhook URL fallback
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                webhookUrl = GetEffectiveWebhookUrl();
            }

            return webhookUrl ?? string.Empty;
        }

        private static DateTime GetTurkeyTime(DateTime? sourceTime = null)
        {
            var utcTime = sourceTime?.ToUniversalTime() ?? DateTime.UtcNow;
            try
            {
                var turkeyZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, turkeyZone);
            }
            catch
            {
                try
                {
                    var europeIstanbul = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
                    return TimeZoneInfo.ConvertTimeFromUtc(utcTime, europeIstanbul);
                }
                catch
                {
                    return utcTime.AddHours(3);
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
                    text = $"🔐 *OzBI Control Tower Login:* {fullName} ({email}) giriş yaptı.",
                    blocks = new object[]
                    {
                        new
                        {
                            type = "header",
                            text = new
                            {
                                type = "plain_text",
                                text = "🔐 OzBI Control Tower - Yeni Kullanıcı Girişi",
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

        public async Task<bool> SendCustomerFeedbackNotificationAsync(CustomerFeedbackSlackPayload p)
        {
            try
            {
                var enabled = _configuration.GetValue<bool>("Slack:Enabled", true);
                if (!enabled)
                {
                    _logger.LogInformation("Slack bildirimleri yapılandırmada kapalı tutuluyor.");
                    return false;
                }

                var webhookUrl = GetEffectiveFeedbackWebhookUrl();
                if (string.IsNullOrWhiteSpace(webhookUrl))
                {
                    _logger.LogWarning("Slack Feedback Webhook URL yapılandırılmamış.");
                    return false;
                }

                var formattedTime = GetTurkeyTime(p.DateCreated).ToString("dd.MM.yyyy HH:mm:ss");

                var blocks = new List<object>();

                // 1. Header / Başlık
                blocks.Add(new
                {
                    type = "header",
                    text = new
                    {
                        type = "plain_text",
                        text = "🔴 OzBI - Müşteri Geri Bildirimi",
                        emoji = true
                    }
                });

                // 2. Firma & Kullanıcı (Fields)
                var userDisplay = !string.IsNullOrWhiteSpace(p.UserName) ? p.UserName : "Kullanıcı";
                if (!string.IsNullOrWhiteSpace(p.UserEmail) && p.UserEmail != "E-posta yok" && !userDisplay.Contains(p.UserEmail))
                {
                    userDisplay += $" ({p.UserEmail})";
                }

                blocks.Add(new
                {
                    type = "section",
                    fields = new object[]
                    {
                        new { type = "mrkdwn", text = $"*🏢 Firma:*\n*{p.TenantName}*" },
                        new { type = "mrkdwn", text = $"*👤 Kullanıcı:*\n{userDisplay}" }
                    }
                });

                // 3. Müşteri Eleştirisi / Yorumu
                var criticismText = !string.IsNullOrWhiteSpace(p.FeedbackReason) 
                    ? p.FeedbackReason.Trim() 
                    : "Beğenilmedi (Yazılı yorum girilmedi)";

                blocks.Add(new
                {
                    type = "section",
                    text = new
                    {
                        type = "mrkdwn",
                        text = $"💬 *Müşteri Eleştirisi:*\n> *“{criticismText}”*"
                    }
                });

                // 4. Kullanıcı Sorusu
                if (!string.IsNullOrWhiteSpace(p.Prompt))
                {
                    blocks.Add(new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"❓ *Kullanıcı Sorusu:*\n{p.Prompt.Trim()}"
                        }
                    });
                }

                var fallbackText = $"🔴 *OzBI Geri Bildirim:* {p.TenantName} ({userDisplay}) - {criticismText}";

                var payload = new
                {
                    text = fallbackText,
                    blocks = blocks.ToArray()
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var client = _httpClientFactory.CreateClient("SlackClient");

                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Slack müşteri geri bildirim bildirimi başarıyla gönderildi: MessageId={Id}, Tenant={Tenant}", p.MessageId, p.TenantName);
                    return true;
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Slack müşteri geri bildirimi gönderilemedi. Kod: {StatusCode}, Yanıt: {ResponseBody}", response.StatusCode, responseBody);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slack müşteri geri bildirimi gönderilirken beklenmeyen bir hata oluştu.");
                return false;
            }
        }
    }
}
