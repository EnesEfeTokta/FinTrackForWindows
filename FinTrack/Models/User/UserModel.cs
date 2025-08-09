namespace FinTrackForWindows.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string MembershipPlan { get; set; } = string.Empty;
    }
}
