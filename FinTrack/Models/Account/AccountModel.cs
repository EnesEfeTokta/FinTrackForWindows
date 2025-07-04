using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Enums;
using System.Windows.Media;

namespace FinTrack.Models.Account
{
    public partial class AccountModel : ObservableObject
    {
        [ObservableProperty]
        private Guid id = Guid.NewGuid();

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private AccountType type;

        [ObservableProperty]
        private decimal currentBalance;

        [ObservableProperty]
        private decimal? targetBalance;

        [ObservableProperty]
        private string currency = "USD";

        [ObservableProperty]
        private List<AccountBalanceHistoryPoint> history = new();

        public string IconPath => Type switch
        {
            AccountType.Checking => "/Assets/Images/Icons/bank.png",
            AccountType.CreditCard => "/Assets/Images/Icons/credit_card.png",
            AccountType.Loan => "/Assets/Images/Icons/investment.png",
            _ => string.Empty
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
                if (Type == AccountType.CreditCard && TargetBalance.HasValue)
                {
                    return $"Kullanılan Limit: {CurrentBalance:C} / {TargetBalance.Value:C}";
                }
                if (TargetBalance.HasValue && TargetBalance > 0)
                {
                    return $"{CurrentBalance:C} / {TargetBalance.Value:C}";
                }
                return $"Değer: {CurrentBalance:C}";
            }
        }
        public double ProgressValue
        {
            get
            {
                if (TargetBalance.HasValue && TargetBalance.Value > 0)
                {
                    return (double)Math.Max(0, Math.Min(100, (CurrentBalance / TargetBalance.Value) * 100));
                }

                return Type == AccountType.Loan ? 100 : 0;
            }
        }

    }
}
