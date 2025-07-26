using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.BudgetDtos
{
    public class BudgetCategoryUpdateDto
    {
        public int BudgetId { get; set; }
        public int CategoryId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public BaseCurrencyType Currency { get; set; }
    }
}
