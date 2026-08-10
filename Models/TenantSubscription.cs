using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("TenantSubscriptions")]
    public class TenantSubscription
    {
        [Key]
        public string TenantId { get; set; } = string.Empty;

        public DateTime? SubscriptionEndDate { get; set; }

        public string? SourceCampaign { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
