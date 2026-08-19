using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("FeedbackPushSnapshots")]
    public class FeedbackPushSnapshot
    {
        [Key]
        public string MessageId { get; set; } = string.Empty;

        public string? ChatId { get; set; }

        public string? TenantName { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? FeedbackReason { get; set; }

        public bool? IsLiked { get; set; }

        public DateTime PushedAt { get; set; } = DateTime.UtcNow;

        public string? PushedBy { get; set; } = "AutoDaemon"; // "AutoDaemon" or Portal User name

        public string Status { get; set; } = "Success"; // "Success" or Error message
    }

    public class CustomerFeedbackSlackPayload
    {
        public string MessageId { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string TenantName { get; set; } = "Bilinmeyen Firma";
        public string UserName { get; set; } = "Kullanıcı";
        public string UserEmail { get; set; } = "E-posta yok";
        public string? AIModelName { get; set; }
        public string? AssistantName { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? IsLiked { get; set; }
        public string? FeedbackReason { get; set; }
        public string? Prompt { get; set; }
        public string? GeneratedSql { get; set; }
        public string? AIResponse { get; set; }
        public string? ErrorMessage { get; set; }
        public long? DurationMs { get; set; }
        public string? PushedBy { get; set; }
    }
}
