using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("FavoriteItems")]
    public class FavoriteItem
    {
        [Key]
        public int Id { get; set; }

        public int PortalUserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ItemType { get; set; } = "Tenant"; // "Tenant" or "User"

        [Required]
        [MaxLength(100)]
        public string ItemId { get; set; } = string.Empty;

        [MaxLength(250)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? ItemSubText { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
