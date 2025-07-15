using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Models.Notification
{
    public partial class NotificationModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string message = string.Empty;

        [ObservableProperty]
        private string timestamp = string.Empty;

        [ObservableProperty]
        private NotificationType type;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUnread))]
        private bool isRead = false;

        public bool IsUnread => !IsRead;

        public NotificationModel(string title, string message, string? timestamp, NotificationType type, bool _isRead = false)
        {
            Title = title;
            Message = message;
            Type = type;
            Timestamp = timestamp ?? DateTime.Now.ToString();
            IsRead = _isRead;
        }
    }
}
