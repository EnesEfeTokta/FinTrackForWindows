using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Models
{
    [Table("BudgetCategories")]
    public class BudgetCategoryModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Column("Budget")]
        public int BudgetId { get; set; }
        [ForeignKey("Budget")]
        public virtual BudgetModel Budget { get; set; } = null!;

        [Required]
        [Column("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Category")]
        public virtual CategoryModel Category { get; set; } = null!;

        [Required]
        [Column("AllocatedAmount", TypeName = "decimal(18, 2)")]
        [Range(0.00, (double)decimal.MaxValue)]
        public decimal AllocatedAmount { get; set; }
    }
}
