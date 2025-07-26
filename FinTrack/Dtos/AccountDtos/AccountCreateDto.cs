using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.AccountDtos
{
    public class AccountCreateDto
    {
        public string Name { get; set; } = null!;
        public AccountType Type { get; set; }
        public bool IsActive { get; set; }
        public BaseCurrencyType Currency { get; set; }
    }
}
