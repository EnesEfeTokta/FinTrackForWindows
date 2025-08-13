using FinTrackForWindows.Views;

namespace FinTrackForWindows.Services.Dialog
{
    public class WpfDialogService : IDialogService
    {
        public string? ShowOtpDialog(string newEmail)
        {
            var dialog = new OtpInputDialogWindow(newEmail);
            if (dialog.ShowDialog() == true)
            {
                return dialog.OtpCode;
            }
            return null;
        }
    }
}
