namespace FinTrackForWindows.Dtos.AccountDtos
{
    public class AccountResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
