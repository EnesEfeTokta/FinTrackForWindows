using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using FinTrack.Models.Notification;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrack.ViewModels
{
    public partial class NotificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<NotificationModel> notifications;

        private readonly ILogger<NotificationViewModel> _logger;

        public NotificationViewModel(ILogger<NotificationViewModel> logger)
        {
            _logger = logger;
            LoadSampleNotifications();
        }

        // TODO: [TEST]
        private void LoadSampleNotifications()
        {
            Notifications = new ObservableCollection<NotificationModel>
            {
                new NotificationModel
                (
                    "Yeni Bütçe Önerisi",
                    "Aylık 'Eğlence' harcamalarınız için yeni bir bütçe limiti önerimiz var. Göz atmak için tıklayın.",
                    "2 saat önce",
                    NotificationType.Suggestion,
                    false
                ),
                new NotificationModel
                (
                    "Hedefe Ulaşıldı!",
                    "'Yeni Bilgisayar' birikim hedefinize ulaştınız. Tebrikler!",
                    "1 gün önce",
                    NotificationType.GoalAchieved,
                    true
                ),
                new NotificationModel
                (
                    "Fatura Hatırlatması",
                    "İnternet faturanızın son ödeme tarihi yarın. Gecikme faizinden kaçınmak için ödeme yapın.",
                    "3 gün önce",
                    NotificationType.Warning,
                    false
                )
            };
        }

        [RelayCommand]
        private void MarkAllAsRead()
        {
            foreach (var notification in Notifications)
            {
                if (notification.IsUnread)
                {
                    notification.IsRead = true;
                    _logger.LogInformation($"Notification '{notification.Title}' marked as read.");
                }
            }
            _logger.LogInformation("All notifications marked as read.");
        }

        [RelayCommand]
        private void ClearAll()
        {
            Notifications.Clear();
            _logger.LogInformation("All notifications cleared.");
        }

        [RelayCommand]
        private void MarkAsRead(NotificationModel? notification)
        {
            if (notification.IsUnread)
            {
                notification.IsRead = true;
                _logger.LogInformation($"Notification '{notification.Title}' marked as read.");
            }
        }

        [RelayCommand]
        private void DeleteNotification(NotificationModel? notification)
        {
            if (notification != null)
            {
                Notifications.Remove(notification);
                _logger.LogInformation($"Notification '{notification.Title}' deleted.");
            }
        }
    }
}
