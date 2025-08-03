namespace FinTrackForWindows.Dtos.MembershipDtos
{
    public class UserMembershipDto
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool AutoRenew { get; set; }
    }
}
