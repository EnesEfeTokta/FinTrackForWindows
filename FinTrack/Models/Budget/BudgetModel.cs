using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Globalization;

namespace FinTrackForWindows.Models.Budget
{
    public partial class BudgetModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private string category = "Other";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressValue))]
        [NotifyPropertyChangedFor(nameof(ProgressText))]
        private decimal allocatedAmount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressValue))]
        [NotifyPropertyChangedFor(nameof(ProgressText))]
        private decimal currentAmount;

        [ObservableProperty]
        private BaseCurrencyType currency = BaseCurrencyType.USD;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemainingTimeText))]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemainingTimeText))]
        private DateTime endDate = DateTime.Today.AddMonths(1);

        public double ProgressValue => AllocatedAmount <= 0 ? 0 : Math.Max(0, Math.Min(100, (double)(CurrentAmount / AllocatedAmount) * 100));

        public string ProgressText
        {
            get
            {
                var culture = new CultureInfo("tr-TR");
                return $"{CurrentAmount.ToString("C", culture)} / {AllocatedAmount.ToString("C", culture)}";
            }
        }

        public string RemainingTimeText
        {
            get
            {
                var remaining = EndDate.Date - DateTime.Today;
                if (remaining.TotalDays < 0)
                    return "Süre Doldu";
                if (remaining.TotalDays == 0)
                    return "Son gün";
                return $"{remaining.Days} gün kaldı";
            }
        }
    }
}