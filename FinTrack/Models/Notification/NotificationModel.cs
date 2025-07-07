using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Enums;

namespace FinTrack.Models.Notification
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
        [NotifyPropertyChangedFor(nameof(isUnread))]
        private bool isRead = false;

        public bool isUnread => !isRead;

        public NotificationModel(string title, string message, string? timestamp, NotificationType type, bool _isRead = false)
        {
            Title = title;
            Message = message;
            Type = type;
            Timestamp = timestamp ?? DateTime.Now.ToString();
            isRead = _isRead;
        }
    }
}
