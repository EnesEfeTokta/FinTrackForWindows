using FinTrack.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    [Table("Accounts")]
    public class AccountModel
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; } = null!;

        [Required]
        [Column("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("AccountType")]
        public AccountType Type { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("Balance")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Balance must be greater than zero.")]
        public decimal Balance { get; set; } = 0;

        [Required]
        [DataType(DataType.DateTime)]
        [Column("CreateAt")]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        [Column("UpdateAt")]
        public DateTime? UpdatedAtUtc { get; set; }

        [Required]
        [Column("IsSynced")]
        public bool IsSynced { get; set; } = false;

        public virtual ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
    }
}
