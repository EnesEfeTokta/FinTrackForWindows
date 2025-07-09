using System.ComponentModel;

namespace FinTrack.Enums
{
    public enum BaseCurrencyType
    {
        [Description("TRY - Turkish Lira")]
        TRY,

        [Description("USD - United States Dollar")]
        USD,

        [Description("EUR - Euro")]
        EUR,

        [Description("GBP - British Pound")]
        GBP,

        [Description("JPY - Japanese Yen")]
        JPY,

        [Description("AUD - Australian Dollar")]
        AUD,

        [Description("CAD - Canadian Dollar")]
        CAD,

        [Description("CHF - Swiss Franc")]
        CHF
    }
}
