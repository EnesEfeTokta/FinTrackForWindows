using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.FeedbackDtos
{
    public class FeedbackCreateDto
    {
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeedbackType? Type { get; set; }
        public string? SavedFilePath { get; set; }
    }
}
