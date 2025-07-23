using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.TransactionDtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public TransactionCategoriesDto Category { get; set; } = null!;
        public AccountDto Account { get; set; } = null!;
        public decimal Amount { get; set; }
        public BaseCurrencyType Currency { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
