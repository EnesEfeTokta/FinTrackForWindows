using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Windows.Media;

namespace FinTrackForWindows.Models.Transaction
{
    public partial class TransactionModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string nameOrDescription = string.Empty;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [ObservableProperty]
        private int accountId;

        [ObservableProperty]
        private string accountName = string.Empty;

        [ObservableProperty]
        private int categoryId;

        [ObservableProperty]
        private string categoryName = string.Empty;

        [ObservableProperty]
        private BaseCurrencyType currency;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IconBackground))]
        [NotifyPropertyChangedFor(nameof(AmountForegroundColor))]
        private TransactionType type;

        private static readonly Brush IncomeBrush = new SolidColorBrush(Color.FromRgb(34, 197, 94));
        private static readonly Brush ExpenseBrush = new SolidColorBrush(Color.FromRgb(239, 68, 68));
        private static readonly Brush DefaultBrush = new SolidColorBrush(Colors.Gray);

        public Brush IconBackground => Type switch
        {
            TransactionType.Income => IncomeBrush,
            TransactionType.Expense => ExpenseBrush,
            _ => DefaultBrush
        };

        public Brush AmountForegroundColor => Type switch
        {
            TransactionType.Income => IncomeBrush,
            TransactionType.Expense => ExpenseBrush,
            _ => DefaultBrush
        };
    }
}