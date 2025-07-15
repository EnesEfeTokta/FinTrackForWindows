using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Windows.Media;

namespace FinTrackForWindows.Models.Transaction
{
    public partial class TransactionModel : ObservableObject
    {
        [ObservableProperty]
        private Guid id = Guid.NewGuid();

        [ObservableProperty]
        private string nameOrDescription = string.Empty;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [ObservableProperty]
        private string accountName = string.Empty;

        [ObservableProperty]
        private string category = string.Empty;

        [ObservableProperty]
        private string currency = "USD";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IconBackground))]
        [NotifyPropertyChangedFor(nameof(AmountForegroundColor))]
        private TransactionType type;

        private static readonly Brush IncomeBrush = new SolidColorBrush(Colors.Green);
        private static readonly Brush ExpenseBrush = new SolidColorBrush(Colors.Red);
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
