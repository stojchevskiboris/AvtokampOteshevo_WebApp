using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_IT.Models
{
    [Table("SystemSettings")]
    public class SystemSetting
    {
        [Key]
        [StringLength(100)]
        public string Key { get; set; } // e.g., "BookingPrices", "BannerNotice"

        [Required]
        public string Value { get; set; } // Stores JSON string or plain text values

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}
