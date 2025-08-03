namespace FinTrackForWindows.Dtos.CurrencyDtos
{
    public class ConvertResponseDto
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Result { get; set; }
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
