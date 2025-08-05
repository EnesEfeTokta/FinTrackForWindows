namespace FinTrackForWindows.Services.AppInNotifications
{
    public interface IAppInNotificationService
    {
        void ShowSuccess(string message);
        void ShowInfo(string message);
        void ShowWarning(string message);
        void ShowError(string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
    }
}
