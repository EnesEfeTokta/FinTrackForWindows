using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.TransactionDtos
{
    public class TransactionCategoryCreateDto
    {
        public string Name { get; set; } = null!;
        public TransactionType Type { get; set; }
    }
}
