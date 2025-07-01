namespace FinTrack.Dtos
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string MembershipType { get; set; } = string.Empty;
    }
}
