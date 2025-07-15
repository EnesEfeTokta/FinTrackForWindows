using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace FinTrackForWindows.Models.Budget
{
    public partial class BudgetModel : ObservableObject
    {
        [ObservableProperty]
        private Guid id = Guid.NewGuid();

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressValue))]
        [NotifyPropertyChangedFor(nameof(ProgressText))]
        private decimal amount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressValue))]
        [NotifyPropertyChangedFor(nameof(ProgressText))]
        private decimal targetAmount;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemainingTimeText))]
        private DateTime endDate;

        [ObservableProperty]
        private string currency = "USD";

        public double ProgressValue => TargetAmount <= 0 ? 0 : Math.Max(0, Math.Min(100, (double)(Amount / TargetAmount) * 100));

        public string ProgressText => $"{Amount.ToString("C", new CultureInfo("tr-TR"))} / {TargetAmount.ToString("C", new CultureInfo("tr-TR"))}";

        public string RemainingTimeText
        {
            get
            {
                var remaining = EndDate - DateTime.Today;
                if (remaining.TotalDays < 0)
                    return "Süre Doldu";
                return $"Kalan Süre: {remaining.Days} gün";
            }
        }
    }
}