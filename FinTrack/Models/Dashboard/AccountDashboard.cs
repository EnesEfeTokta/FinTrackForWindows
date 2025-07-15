using System.Windows.Media;

namespace FinTrackForWindows.Models.Dashboard
{
    public class AccountDashboard
    {
        public string Name { get; set; } = string.Empty;
        public double Percentage { get; set; }
        public string Balance { get; set; } = string.Empty;
        public Brush ProgressBarBrush { get; set; } = Brushes.Transparent;
    }
}
