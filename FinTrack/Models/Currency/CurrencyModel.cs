using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Windows.Media;

namespace FinTrackForWindows.Models.Currency
{
    public partial class CurrencyModel : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string toCurrencyFlag = string.Empty;

        [ObservableProperty]
        private string toCurrencyCode = string.Empty;

        [ObservableProperty]
        private string toCurrencyName = string.Empty;

        [ObservableProperty]
        private decimal toCurrencyPrice;

        [ObservableProperty]
        private string toCurrencyChange = "N/A";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToCurrencyChangeForeground))]
        private CurrencyConversionType type = CurrencyConversionType.Neutral;

        [ObservableProperty]
        private string dailyLow = "N/A";

        [ObservableProperty]
        private string dailyHigh = "N/A";

        [ObservableProperty]
        private string weeklyChange = "N/A";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WeeklyChangeForeground))]
        private CurrencyConversionType weeklyChangeType = CurrencyConversionType.Neutral;

        [ObservableProperty]
        private string monthlyChange = "N/A";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MonthlyChangeForeground))]
        private CurrencyConversionType monthlyChangeType = CurrencyConversionType.Neutral;

        private static readonly Brush IncreaseBrush = new SolidColorBrush(Color.FromRgb(46, 204, 113));
        private static readonly Brush DecreaseBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        private static readonly Brush DefaultBrush = new SolidColorBrush(Colors.Gray);

        public Brush ToCurrencyChangeForeground => Type switch
        {
            CurrencyConversionType.Increase => IncreaseBrush,
            CurrencyConversionType.Decrease => DecreaseBrush,
            _ => DefaultBrush
        };

        public Brush WeeklyChangeForeground => WeeklyChangeType switch
        {
            CurrencyConversionType.Increase => IncreaseBrush,
            CurrencyConversionType.Decrease => DecreaseBrush,
            _ => DefaultBrush
        };

        public Brush MonthlyChangeForeground => MonthlyChangeType switch
        {
            CurrencyConversionType.Increase => IncreaseBrush,
            CurrencyConversionType.Decrease => DecreaseBrush,
            _ => DefaultBrush
        };
    }
}