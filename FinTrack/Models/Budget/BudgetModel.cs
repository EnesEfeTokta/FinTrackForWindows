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
        [NotifyPropertyChangedFor(nameof(ProgressValue))]
        [NotifyPropertyChangedFor(nameof(ProgressText))]
        private decimal? reachedAmount;

        [ObservableProperty]
        private bool isEditingReachedAmount = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressText))]
        [NotifyPropertyChangedFor(nameof(ReachedAmountDisplayText))]
        private BaseCurrencyType currency;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemainingTimeText))]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemainingTimeText))]
        private DateTime endDate = DateTime.Today.AddMonths(1);

        public double ProgressValue => AllocatedAmount <= 0 ? 0 : Math.Max(0, Math.Min(100, (double)((ReachedAmount ?? 0) / AllocatedAmount) * 100));

        public string ProgressText
        {
            get
            {
                var current = ReachedAmount ?? 0;
                string currencyCode = Currency.ToString();
                var culture = CultureInfo.CurrentCulture;

                string currentText = current.ToString("N2", culture);
                string allocatedText = AllocatedAmount.ToString("N2", culture);

                return $"{currentText} {currencyCode} / {allocatedText} {currencyCode}";
            }
        }

        public string ReachedAmountDisplayText
        {
            get
            {
                var current = ReachedAmount ?? 0;
                string currencyCode = Currency.ToString();
                var culture = CultureInfo.CurrentCulture;
                return $"{current.ToString("N2", culture)} {currencyCode}";
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