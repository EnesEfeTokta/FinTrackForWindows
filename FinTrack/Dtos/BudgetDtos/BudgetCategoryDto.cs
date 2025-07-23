using FinTrackForWindows.Enums;
using FinTrackForWindows.Models;

namespace FinTrackForWindows.Dtos.BudgetDtos
{
    public class BudgetCategoryDto
    {
        public int Id { get; set; }
        public BudgetModel Budget { get; set; } = null!;
        public CategoryModel Category { get; set; } = null!;
        public decimal AllocatedAmount { get; set; }
        public BaseCurrencyType Currency { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
