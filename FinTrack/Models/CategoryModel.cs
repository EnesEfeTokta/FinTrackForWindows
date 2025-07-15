using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrackForWindows.Models
{
    [Table("Categories")]
    public class CategoryModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Column("User")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; } = null!;

        [Required]
        [Column("CategoryName")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("CategoryType")]
        public CategoryType Type { get; set; }

        [Required]
        [Column("IsSynced")]
        public bool IsSynced { get; set; } = false;

        public virtual ICollection<BudgetCategoryModel> BudgetAllocations { get; set; } = new List<BudgetCategoryModel>();
        public virtual ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
    }

    public enum CategoryType
    {
        Expense = 0,
        Income = 1,
    }
}
