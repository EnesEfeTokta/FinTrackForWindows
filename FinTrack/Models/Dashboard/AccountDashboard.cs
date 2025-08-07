using System.Windows.Media;

namespace FinTrackForWindows.Models.Dashboard
{
    public class AccountDashboard
    {
        public string Name { get; set; } = string.Empty;
        public string Balance { get; set; } = string.Empty;

        public double IncomePercentage { get; set; }
        public double ExpensePercentage { get; set; }

        public string IncomeAmountText { get; set; } = string.Empty;
        public string ExpenseAmountText { get; set; } = string.Empty;
    }
}