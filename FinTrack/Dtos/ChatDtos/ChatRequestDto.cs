namespace FinTrackForWindows.Dtos.ChatDtos
{
    public class ChatRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public string? ClientChatSessionId { get; set; }
    }
}
