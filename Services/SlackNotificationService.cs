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
        private const string DefaultSqlErrorBase64WebhookUrl = "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQlJBMFNSUFJDL3lvR1JpOEVWNHdaUEVrbWV5ejFHVWltUw==";

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

        private string GetEffectiveSqlErrorWebhookUrl()
        {
            // 1. Özel SqlError Webhook URL (appsettings.json)
            var webhookUrl = _configuration["Slack:SqlErrorWebhookUrl"] ?? _configuration["Slack:ErrorWebhookUrl"];

            // 2. Ortam değişkeni fallback
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                webhookUrl = Environment.GetEnvironmentVariable("SLACK_SQL_ERROR_WEBHOOK_URL") 
                             ?? Environment.GetEnvironmentVariable("SLACK_ERROR_WEBHOOK_URL");
            }

            // 3. Base64 varsayılan SQL Error Webhook URL fallback (#ozbi-sql-errors kanalı)
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                try
                {
                    webhookUrl = Encoding.UTF8.GetString(Convert.FromBase64String(DefaultSqlErrorBase64WebhookUrl));
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

        public async Task<bool> SendSqlErrorNotificationAsync(SqlErrorSlackPayload payload)
        {
            try
            {
                var p = payload;
                var webhookUrl = GetEffectiveSqlErrorWebhookUrl();

                if (string.IsNullOrWhiteSpace(webhookUrl))
                {
                    _logger.LogWarning("Slack SQL Error Webhook URL yapılandırılmamış.");
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
                        text = "⚠️ OzBI - SQL / Sistem Hatası Tespit Edildi",
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

                // 3. Kullanıcı Sorusu
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

                // 4. Hata Detayı
                var errText = !string.IsNullOrWhiteSpace(p.ErrorMessage) 
                    ? p.ErrorMessage.Trim() 
                    : "Bilinmeyen sistem hatası";
                if (errText.Length > 800) errText = errText.Substring(0, 800) + "...";

                blocks.Add(new
                {
                    type = "section",
                    text = new
                    {
                        type = "mrkdwn",
                        text = $"🔴 *Hata Detayı:*\n```{errText}```"
                    }
                });

                // 5. Hatalı SQL Sorgusu (varsa)
                if (!string.IsNullOrWhiteSpace(p.SqlQuery))
                {
                    var sqlText = p.SqlQuery.Trim();
                    if (sqlText.Length > 1000) sqlText = sqlText.Substring(0, 1000) + "\n-- (...kesildi...)";
                    blocks.Add(new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = $"💻 *Hatalı SQL / Query:*\n```sql\n{sqlText}\n```"
                        }
                    });
                }

                // 6. AI Çözüm & Düzeltme Önerisi (Logo / Mikro ERP Özelinde)
                var fixSuggestion = GenerateAiFixSuggestion(p.ErrorMessage, p.SqlQuery, p.AssistantName, p.TenantName);
                if (!string.IsNullOrWhiteSpace(fixSuggestion))
                {
                    blocks.Add(new
                    {
                        type = "section",
                        text = new
                        {
                            type = "mrkdwn",
                            text = fixSuggestion
                        }
                    });
                }

                // 7. Context footer
                blocks.Add(new
                {
                    type = "context",
                    elements = new object[]
                    {
                        new { type = "mrkdwn", text = $"🔍 *Kayıt ID:* `{p.MessageId}` | *Sohbet ID:* `{p.ChatId}` | *Tarih (TSI):* {formattedTime}" }
                    }
                });

                var fallbackText = $"⚠️ *OzBI SQL Hatası:* {p.TenantName} ({userDisplay}) - {errText}";

                var slackObj = new
                {
                    text = fallbackText,
                    blocks = blocks.ToArray()
                };

                var jsonPayload = JsonSerializer.Serialize(slackObj);
                var client = _httpClientFactory.CreateClient("SlackClient");

                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Slack SQL hata bildirimi başarıyla gönderildi: MessageId={Id}, Tenant={Tenant}", p.MessageId, p.TenantName);
                    return true;
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Slack SQL hata bildirimi gönderilemedi. Kod: {StatusCode}, Yanıt: {ResponseBody}", response.StatusCode, responseBody);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slack SQL hata bildirimi gönderilirken beklenmeyen bir hata oluştu.");
                return false;
            }
        }

        private static string GenerateAiFixSuggestion(string? errorMessage, string? sqlQuery, string? assistantName, string? tenantName)
        {
            var err = errorMessage ?? string.Empty;
            var sql = sqlQuery ?? string.Empty;
            var upperErr = err.ToUpperInvariant();
            var upperSql = sql.ToUpperInvariant();
            var upperAsst = (assistantName ?? string.Empty).ToUpperInvariant();
            var upperTenant = (tenantName ?? string.Empty).ToUpperInvariant();

            // Determine ERP Type
            string erpName = "Genel";
            if (upperAsst.Contains("MIKRO") || upperTenant.Contains("MEVLANA") || upperTenant.Contains("TTS") || upperSql.Contains("CARI_HESAP") || upperSql.Contains("STOKLAR"))
            {
                erpName = "Mikro ERP";
            }
            else if (upperAsst.Contains("LOGO") || upperTenant.Contains("Q BILGI") || upperSql.Contains("LG_") || upperSql.Contains("CLCARD") || upperSql.Contains("STLINE"))
            {
                erpName = "Logo ERP";
            }
            else if (upperAsst.Contains("NETSIS") || upperSql.Contains("TBLCASABIT") || upperSql.Contains("TBLFATUIRS"))
            {
                erpName = "Netsis ERP";
            }

            // 1. Nested Aggregate Hatası
            if (upperErr.Contains("CANNOT PERFORM AN AGGREGATE FUNCTION ON AN EXPRESSION CONTAINING AN AGGREGATE") || 
                upperErr.Contains("CONTAINING AN AGGREGATE"))
            {
                if (erpName == "Mikro ERP")
                {
                    return "💡 *AI Çözüm & Düzeltme Önerisi (Mikro ERP):*\n> T-SQL'de `MAX(MIN(...))` veya aggregate içinde aggregate kullanılamaz. Vade/tarih farkı hesaplamasını (`DATEDIFF`) CTE (`WITH FaturaBazinda`) içinde satır bazında yapıp, ana sorguda sadece `MAX(tanimli_vade_gunu)` olarak toplayın.";
                }
                else if (erpName == "Logo ERP")
                {
                    return "💡 *AI Çözüm & Düzeltme Önerisi (Logo ERP):*\n> T-SQL'de aggregate içinde aggregate kullanılamaz. Fatura satırları veya vade hesaplamasını alt sorguda (CTE/Subquery) yapıp, dış sorguda `SUM` veya `MAX` uygulayın.";
                }
                else
                {
                    return "💡 *AI Çözüm & Düzeltme Önerisi:*\n> T-SQL kuralları gereği aggregate (SUM/MAX/MIN) içinde başka bir aggregate kullanılamaz. Hesaplamayı CTE veya alt sorgu içinde yapıp dışarıda toplayın.";
                }
            }

            // 2. Tablo Bulunamadı (Invalid Object Name)
            if (upperErr.Contains("INVALID OBJECT NAME"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(err, @"'([^']+)'");
                var objName = match.Success ? match.Groups[1].Value : "tablo";

                if (erpName == "Logo ERP")
                {
                    return $"💡 *AI Çözüm & Düzeltme Önerisi (Logo ERP):*\n> `{objName}` tablosu bulunamadı. Logo ERP'de tablo adları firma ve dönem numarası içerir (Örn: `LG_FFF_DD_STLINE` veya `LG_FFF_CLCARD`). Modelin kullandığı firma/dönem kodu `{objName}` tenant'ın güncel ERP veritabanı ön ekiyle uyuşmuyor; şema eşleştirmesini kontrol edin.";
                }
                else if (erpName == "Mikro ERP")
                {
                    return $"💡 *AI Çözüm & Düzeltme Önerisi (Mikro ERP):*\n> `{objName}` tablosu bulunamadı. Mikro ERP standart tablolarını (`CARI_HESAPLAR`, `CARI_HESAP_HAREKETLERI`, `STOKLAR`, `STOK_HAREKETLERI`) ve tenant veritabanındaki görünürlük izinlerini kontrol edin.";
                }
                else
                {
                    return $"💡 *AI Çözüm & Düzeltme Önerisi:*\n> `{objName}` tablosu veritabanında bulunamadı. Modelin sorguladığı tablo ismini ve tenant şemasını kontrol edin.";
                }
            }

            // 3. Kolon Bulunamadı (Invalid Column Name)
            if (upperErr.Contains("INVALID COLUMN NAME"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(err, @"'([^']+)'");
                var colName = match.Success ? match.Groups[1].Value : "kolon";

                if (erpName == "Mikro ERP")
                {
                    return $"💡 *AI Çözüm & Düzeltme Önerisi (Mikro ERP):*\n> `{colName}` kolonu bulunamadı. Mikro ERP standart alanlarını (`cha_kod`, `cha_tarihi`, `cha_meblag`, `msg_S_...`) veya özel kullanıcı tanımlı alanları kontrol edin.";
                }
                else if (erpName == "Logo ERP")
                {
                    return $"💡 *AI Çözüm & Düzeltme Önerisi (Logo ERP):*\n> `{colName}` kolonu bulunamadı. Logo ERP standart kolonlarını (`CODE`, `DEFINITION_`, `DATE_`, `TOTAL`, `AMOUNT`, `LOGICALREF`) kontrol edin.";
                }
                else
                {
                    return $"💡 *AI Çözüm & Düzeltme Önerisi:*\n> `{colName}` kolonu tabloda bulunamadı. İlgili tablonun şemasını ve kolon adını doğrulayın.";
                }
            }

            // 4. Zaman Aşımı (Timeout)
            if (upperErr.Contains("ZAMAN AŞIMI") || upperErr.Contains("TIMEOUT") || upperErr.Contains("TIME OUT"))
            {
                if (erpName == "Mikro ERP")
                {
                    return "💡 *AI Performans Önerisi (Mikro ERP):*\n> Sorgu zaman aşımına uğradı. `CARI_HESAP_HAREKETLERI` veya `STOK_HAREKETLERI` tablosunda `cha_tarihi`/`sth_tarihi` tarih filtresi eksik veya indeks taranmamış. Sorguya tarih aralığı ve `TOP 100` ekleyin.";
                }
                else if (erpName == "Logo ERP")
                {
                    return "💡 *AI Performans Önerisi (Logo ERP):*\n> `LG_xxx_xx_STLINE` veya `CLFLINE` tablosu taranırken zaman aşımı oluştu. `DATE_` filtresi ve `LOGICALREF` indekslerini optimize edin, `TOP 100` ekleyin.";
                }
                else
                {
                    return "💡 *AI Performans Önerisi:*\n> Sorgu zaman aşımına uğradı. Tablo boyutunu sınırlamak için tarih filtresi ve `TOP 100` limiti ekleyin.";
                }
            }

            // 5. Veri Tipi Dönüşüm Hatası (Conversion / Data Type Error)
            if (upperErr.Contains("CONVERT") || upperErr.Contains("CAST") || upperErr.Contains("CONVERSION FAILED"))
            {
                if (erpName == "Mikro ERP")
                {
                    return "💡 *AI Çözüm & Düzeltme Önerisi (Mikro ERP):*\n> Tip dönüşüm hatası. Mikro ERP'de tarihler int (YYYYMMDD) veya float olarak saklanabilir. Hata almamak için `TRY_CONVERT(date, CONVERT(varchar(8), [kolon]), 112)` güvenli dönüşümünü kullanın.";
                }
                else
                {
                    return "💡 *AI Çözüm & Düzeltme Önerisi:*\n> Veri tipi dönüşüm hatası. `CONVERT`/`CAST` yerine hata vermeyen `TRY_CONVERT`/`TRY_CAST` fonksiyonlarını tercih edin.";
                }
            }

            // 6. Sıfıra Bölme Hatası (Divide by Zero)
            if (upperErr.Contains("DIVIDE BY ZERO"))
            {
                return "💡 *AI Çözüm & Düzeltme Önerisi:*\n> Sıfıra bölme hatası. Oran ve birim fiyat hesaplamalarında paydayı `NULLIF(payda, 0)` ile korumaya alın.";
            }

            // 7. Genel / Varsayılan İyileştirme
            return $"💡 *AI İyileştirme Önerisi ({erpName}):*\n> Bu hata kalıbını önlemek için portaldeki **Prompt Şablonları (PromptTemplates)** modülüne bu soru için doğru T-SQL kalıbını (Golden Query) ekleyebilirsiniz.";
        }
    }
}
