namespace FinTrack.Models.Dashboard
{
    public class ReportDashboard
    {
        public string Name { get; set; } = string.Empty;
        public string[] Formats { get; set; } = { "PDF", "WORD", "TEXT", "XML", "EXCEL", "MD" };
    }
}
