using Notifications.Wpf;
using System.Windows;

namespace FinTrackForWindows.Services.AppInNotifications
{
    public class AppInNotificationService : IAppInNotificationService
    {
        private readonly NotificationManager _notificationManager;

        public AppInNotificationService()
        {
            _notificationManager = new NotificationManager();
        }

        public void ShowError(string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Error",
                Message = message,
                Type = NotificationType.Error
            });
        }

        public void ShowInfo(string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Information",
                Message = message,
                Type = NotificationType.Information
            });
        }

        public void ShowSuccess(string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Success",
                Message = message,
                Type = NotificationType.Success
            });
        }

        public void ShowWarning(string message)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Warning",
                Message = message,
                Type = NotificationType.Warning
            });
        }


        public void ShowError(string message, Exception exception)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Error",
                Message = $"{message}\n{exception.Message}",
                Type = NotificationType.Error
            });
        }

        public void ShowInfo(string message, Exception exception)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Information",
                Message = $"{message}\n{exception.Message}",
                Type = NotificationType.Information
            });
        }

        public void ShowSuccess(string message, Exception exception)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Success",
                Message = $"{message}\n{exception.Message}",
                Type = NotificationType.Success
            });
        }

        public void ShowWarning(string message, Exception exception)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = "Warning",
                Message = $"{message}\n{exception.Message}",
                Type = NotificationType.Warning
            });
        }

        public void Dispose()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType().Name == "NotificationWindow")
                {
                    try
                    {
                        window.Close();
                    }
                    catch { /* Pencere zaten kapanmış */ }
                }
            }
        }
    }
}
