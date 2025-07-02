namespace FinTrack.Models.Dashboard
{
    public class ReportDashboard
    {
        public string Name { get; set; } = string.Empty;
        public string IconPath { get; set; } = "/Assets/Icons/report.png";
        public string[] Formats { get; set; } = { "PDF", "WORD", "TEXT", "XML", "EXCEL", "MD" };
    }
}
