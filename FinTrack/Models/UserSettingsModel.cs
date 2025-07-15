using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrackForWindows.Models
{
    [Table("UserSettings")]
    public class UserSettingsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; } = null!;

        [Required]
        [Column("Theme")]
        [StringLength(50)]
        public string Theme { get; set; } = "Light";

        [Required]
        [Column("Language")]
        [StringLength(50)]
        public string Language { get; set; } = "English";

        [Required]
        [Column("Currency")]
        [StringLength(10)]
        public string Currency { get; set; } = "USD";

        [Required]
        [Column("Notification")]
        public bool Notification { get; set; } = true;

        [Required]
        [Column("EntryDate")]
        [DataType(DataType.DateTime)]
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("IsSynced")]
        public bool IsSynced { get; set; } = false;
    }
}
