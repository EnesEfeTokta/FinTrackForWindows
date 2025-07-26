using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Windows.Media;

namespace FinTrackForWindows.Models.Account
{
    public partial class AccountModel : ObservableObject
    {
        [ObservableProperty]
        private int? id;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private AccountType type;

        [ObservableProperty]
        private BaseCurrencyType currency;

        [ObservableProperty]
        private List<AccountBalanceHistoryPoint> history = new();

        public decimal? balance { get; set; }

        public string IconPath => Type switch
        {
            AccountType.Checking => "/Assets/Images/Icons/bank.png",
            AccountType.CreditCard => "/Assets/Images/Icons/credit-card.png",
            AccountType.Loan => "/Assets/Images/Icons/investment.png",
            _ => "/Assets/Images/Icons/money.png"
        };

        public Brush IconBackground => Type switch
        {
            AccountType.Checking => new SolidColorBrush(Colors.Green),
            AccountType.CreditCard => new SolidColorBrush(Colors.Red),
            AccountType.Loan => new SolidColorBrush(Colors.DodgerBlue),
            _ => new SolidColorBrush(Colors.Gray)
        };

        public string BalanceText
        {
            get
            {
                var balanceString = balance.ToString();
                return Type switch
                {
                    AccountType.Checking => $"Bakiye: {balance}",
                    AccountType.CreditCard => $"Borç: {balance}",
                    AccountType.Loan => $"Kredi: {balance}",
                    _ => $"Bakiye: {balance}"
                };
            }
        }
    }
}
