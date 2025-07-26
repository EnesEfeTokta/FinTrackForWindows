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
        private string toCurrencyChange = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToCurrencyChangeForeground))]
        private CurrencyConversionType type = CurrencyConversionType.Increase;

        [ObservableProperty]
        private string dailyLow = string.Empty;

        [ObservableProperty]
        private string dailyHigh = string.Empty;

        [ObservableProperty]
        private string weeklyChange = string.Empty;

        [ObservableProperty]
        private string monthlyChange = string.Empty;

        private static readonly Brush IncreaseBrush = new SolidColorBrush(Colors.Green);
        private static readonly Brush DecreaseBrush = new SolidColorBrush(Colors.Red);
        private static readonly Brush DefaultBrush = new SolidColorBrush(Colors.Gray);

        public Brush ToCurrencyChangeForeground => Type switch
        {
            CurrencyConversionType.Increase => IncreaseBrush,
            CurrencyConversionType.Decrease => DecreaseBrush,
            _ => DefaultBrush
        };
    }
}
