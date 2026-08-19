using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("SqlErrorPushSnapshots")]
    public class SqlErrorPushSnapshot
    {
        [Key]
        public string MessageId { get; set; } = string.Empty;

        public string? ChatId { get; set; }

        public string? TenantName { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? ErrorMessage { get; set; }

        public string? Prompt { get; set; }

        public string? SqlQuery { get; set; }

        public DateTime PushedAt { get; set; } = DateTime.UtcNow;

        public string? PushedBy { get; set; } = "AutoDaemon";

        public string Status { get; set; } = "Success";
    }

    public class SqlErrorSlackPayload
    {
        public string MessageId { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string TenantName { get; set; } = "Bilinmeyen Firma";
        public string UserName { get; set; } = "Kullanıcı";
        public string UserEmail { get; set; } = "E-posta yok";
        public string? AIModelName { get; set; }
        public string? AssistantName { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Prompt { get; set; }
        public string? SqlQuery { get; set; }
        public string? AIResponse { get; set; }
        public long? DurationMs { get; set; }
        public string? PushedBy { get; set; }
    }
}
