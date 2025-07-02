using System.Windows.Media;

namespace FinTrack.Models.Dashboard
{
    public class BudgetDashboard
    {
        public string Name { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string RemainingTime { get; set; } = string.Empty;
        public Brush StatusBrush { get; set; } = Brushes.Transparent;
    }
}
