using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? email_ForgotPasswordView_TextBox;

        public event Action? NavigateToLoginRequested;

        private readonly ILogger<ForgotPasswordViewModel> _logger;
        private readonly IAppInNotificationService _appInNotificationService;

        public ForgotPasswordViewModel(ILogger<ForgotPasswordViewModel> logger, IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _appInNotificationService = appInNotificationService;

            _logger.LogInformation("ForgotPasswordViewModel initialized.");
        }

        [RelayCommand]
        private void ResetPassword_ForgotPasswordView_Button()
        {

            if (!string.IsNullOrEmpty(Email_ForgotPasswordView_TextBox))
            {
                _appInNotificationService.ShowInfo($"The password reset link has been sent to {Email_ForgotPasswordView_TextBox}.");
                _logger.LogInformation($"Password reset link sent to {Email_ForgotPasswordView_TextBox}.");
            }
            else
            {
                _appInNotificationService.ShowError("Please enter a valid email address.");
                _logger.LogWarning("Password reset attempted with an empty email field.");
            }
        }

        [RelayCommand]
        private void NavigateToLogin_ForgotPasswordView_Button()
        {
            NavigateToLoginRequested?.Invoke();
        }
    }
}
