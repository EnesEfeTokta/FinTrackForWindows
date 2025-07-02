using FinTrack.Enums;
using System.Windows.Media;

namespace FinTrack.Models.Dashboard
{
    public class TransactionDashboard
    {
        public string DateText { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public TransactionType Type { get; set; } = TransactionType.Expense;
        public Brush DateBadgeBrush => Type == TransactionType.Income ? Brushes.Green : Brushes.Red;
    }
}
