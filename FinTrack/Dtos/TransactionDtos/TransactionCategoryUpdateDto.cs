using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.TransactionDtos
{
    public class TransactionCategoryUpdateDto
    {
        public string Name { get; set; } = null!;
        public TransactionType Type { get; set; }
    }
}
