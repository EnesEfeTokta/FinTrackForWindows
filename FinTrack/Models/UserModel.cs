using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    [Table("Users")]
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("ProfilePicture")]
        public string ProfilePicture { get; set; } = "default.png";

        [Required]
        [Column("CreateAt")]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set; }

        [Required]
        [Column("UserName")]
        [StringLength(256)]
        public string? UserName { get; set; }

        [Column("NormalizedUserName")]
        [StringLength(256)]
        public string? NormalizedUserName { get; set; }

        [Required]
        [Column("Email")]
        [StringLength(256)]
        public string? Email { get; set; }

        [Column("NormalizedEmail")]
        [StringLength(256)]
        public string? NormalizedEmail { get; set; }

        [Required]
        [Column("EmailConfirmed")]
        public bool EmailConfirmed { get; set; } = false;

        [Required]
        [Column("PasswordHash")]
        public string? PasswordHash { get; set; }

        [Column("SecurityStamp")]
        [StringLength(256)]
        public string? SecurityStamp { get; set; }

        [Column("ConcurrencyStamp")]
        [StringLength(256)]
        public string? ConcurrencyStamp { get; set; }

        [Column("PhoneNumber")]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Column("PhoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; } = false;

        [Column("TwoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; } = false;

        [Column("LockoutEnd")]
        [DataType(DataType.DateTime)]
        public DateTime? LockoutEnd { get; set; }

        [Column("LockoutEnabled")]
        [Required]
        public bool LockoutEnabled { get; set; } = false;

        [Column("AccessFailedCount")]
        [Required]
        public int AccessFailedCount { get; set; } = 0;

        public virtual UserSettingsModel? UserSettings { get; set; }
        public virtual ICollection<AccountModel> Accounts { get; set; } = new List<AccountModel>();
        public virtual ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
        public virtual ICollection<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public virtual ICollection<BudgetModel> Budgets { get; set; } = new List<BudgetModel>();
        public virtual ICollection<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();
    }
}