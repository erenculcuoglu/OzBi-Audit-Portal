using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("UserLoginSnapshots")]
    public class UserLoginSnapshot
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        public int LastSeenLoginCount { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
