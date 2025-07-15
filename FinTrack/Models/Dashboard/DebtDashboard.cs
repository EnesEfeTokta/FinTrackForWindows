using System.Windows.Media;

namespace FinTrackForWindows.Models.Dashboard
{
    public class DebtDashboard
    {
        public string LenderName { get; set; } = string.Empty;
        public string LenderIconPath { get; set; } = string.Empty;
        public string BorrowerName { get; set; } = string.Empty;
        public string BorrowerIconPath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Brush StatusBrush { get; set; } = Brushes.Transparent;
        public string Amount { get; set; } = string.Empty;
        public string CreationDate { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string ReviewDate { get; set; } = string.Empty;
    }
}
