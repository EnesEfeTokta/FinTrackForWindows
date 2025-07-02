namespace FinTrack.Models.Dashboard
{
    public class CurrencyRateDashboard
    {
        public string FromCurrencyFlagUrl { get; set; } = string.Empty;
        public string FromCurrencyCountry { get; set; } = string.Empty;
        public string FromCurrencyName { get; set; } = string.Empty;
        public string FromCurrencyAmount { get; set; } = string.Empty;
        public string ToCurrencyFlagUrl { get; set; } = string.Empty;
        public string ToCurrencyCountry { get; set; } = string.Empty;
        public string ToCurrencyName { get; set; } = string.Empty;
        public string ToCurrencyAmount { get; set; } = string.Empty;
        public double ToCurrencyImageHeight { get; set; } = 20;
    }
}
