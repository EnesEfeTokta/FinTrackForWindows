using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.TransactionDtos
{
    public class TransactionCategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
