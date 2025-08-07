namespace FinTrackForWindows.Services.AppInNotifications
{
    public interface IAppInNotificationService: IDisposable
    {
        void ShowSuccess(string message);
        void ShowInfo(string message);
        void ShowWarning(string message);
        void ShowError(string message);

        void ShowError(string message, Exception exception);
        void ShowInfo(string message, Exception exception);
        void ShowSuccess(string message, Exception exception);
        void ShowWarning(string message, Exception exception);
    }
}
