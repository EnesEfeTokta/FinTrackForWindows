using HandyControl.Controls;
using HandyControl.Data;
using System.Windows;

namespace FinTrackForWindows.Services.AppInNotifications
{
    public class HandyControlNotificationService : IAppInNotificationService
    {
        public void ShowSuccess(string message)
        {
            Growl.Success(new GrowlInfo { Message = message, WaitTime = 3, ShowDateTime = false });
        }

        public void ShowInfo(string message)
        {
            Growl.Info(message);
        }

        public void ShowWarning(string message)
        {
            Growl.Warning(new GrowlInfo { Message = message, WaitTime = 5 });
        }

        public void ShowError(string message)
        {
            Growl.Error(new GrowlInfo { Message = message, WaitTime = 7, ShowDateTime = false });
        }

        public Task<bool> ShowConfirmationAsync(string title, string content)
        {
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show(
                messageBoxText: content,
                caption: title,
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question
            );

            bool confirmation = (result == MessageBoxResult.Yes);

            return Task.FromResult(confirmation);
        }
    }
}
