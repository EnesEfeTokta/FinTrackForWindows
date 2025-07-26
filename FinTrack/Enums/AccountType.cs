using System.ComponentModel;

namespace FinTrackForWindows.Enums
{
    public enum AccountType
    {
        [Description("Checking")]
        Checking,

        [Description("Savings")]
        Savings,

        [Description("Credit Card")]
        CreditCard,

        [Description("Cash")]
        Cash,

        [Description("Investment")]
        Investment,

        [Description("Loan")]
        Loan,

        [Description("Other")]
        Other,
    } // Kontrol, Tasarruf, Kredi Kartı, Nakit, Yatırım, Kredi, Diğer
}
