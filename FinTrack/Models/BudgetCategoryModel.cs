using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrackForWindows.Models
{
    [Table("BudgetCategories")]
    public class BudgetCategoryModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Column("BudgetId")]
        public int BudgetId { get; set; }
        [ForeignKey("BudgetId")]
        public virtual BudgetModel Budget { get; set; } = null!;

        [Required]
        [Column("CategoryId")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual CategoryModel Category { get; set; } = null!;

        [Required]
        [Column("AllocatedAmount", TypeName = "decimal(18, 2)")]
        [Range(0.00, (double)decimal.MaxValue)]
        public decimal AllocatedAmount { get; set; }

        [Required]
        [Column("IsSynced")]
        public bool IsSynced { get; set; } = false;
    }
}
