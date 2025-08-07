using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinTrackForWindows.Core;
using FinTrackForWindows.Messages;
using FinTrackForWindows.Services;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? email_LoginView_TextBox;

        [ObservableProperty]
        private string? password_LoginView_TextBox;

        [ObservableProperty]
        private bool isPasswordVisible_LoginView_PasswordBoxAndTextBox = false;

        [ObservableProperty]
        private string eyeIconSource_LoginView_Image = "/Assets/Images/Icons/eyeclose.png";

        public event Action? NavigateToRegisterRequested;
        public event Action? NavigateToForgotPasswordRequested;

        private readonly IAuthService _authService;
        private readonly ISecureTokenStorage _secureTokenStorage;

        private readonly ILogger<LoginViewModel> _logger;

        private readonly IAppInNotificationService _appInNotificationService;

        public LoginViewModel(
            IAuthService authService,
            ILogger<LoginViewModel> logger,
            ISecureTokenStorage secureTokenStorage,
            IAppInNotificationService appInNotificationService)
        {
            _authService = authService;
            _logger = logger;
            _secureTokenStorage = secureTokenStorage;
            _appInNotificationService = appInNotificationService;
        }

        [RelayCommand]
        private async Task Login_LoginView_Button()
        {
            _logger.LogInformation("User attempting to log in. Email: {Email}", Email_LoginView_TextBox);

            if (string.IsNullOrEmpty(Email_LoginView_TextBox) || string.IsNullOrEmpty(Password_LoginView_TextBox))
            {
                _appInNotificationService.ShowInfo("Please enter both your email and password.");
                _logger.LogWarning("Login attempt failed: Email or password field is empty.");
                return;
            }

            string token = await _authService.LoginAsync(Email_LoginView_TextBox, Password_LoginView_TextBox);
            if (string.IsNullOrEmpty(token))
            {
                _appInNotificationService.ShowError("Login failed. Please check your email and password and try again.");
                _logger.LogError("Login attempt failed: Unable to retrieve token. Invalid email or password.");
                return;
            }

            SessionManager.SetToken(token);
            _secureTokenStorage.SaveToken(token);
            _logger.LogInformation("User logged in successfully. Token has been saved.");

            _appInNotificationService.ShowSuccess("Login successful. Redirecting to the main page.");

            WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
        }

        [RelayCommand]
        private void TogglePasswordVisibility_LoginView_Button()
        {
            IsPasswordVisible_LoginView_PasswordBoxAndTextBox = !IsPasswordVisible_LoginView_PasswordBoxAndTextBox;

            EyeIconSource_LoginView_Image = IsPasswordVisible_LoginView_PasswordBoxAndTextBox
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
            _logger.LogInformation("Password visibility toggled. Password is now {0}.", IsPasswordVisible_LoginView_PasswordBoxAndTextBox ? "visible" : "hidden");
        }

        [RelayCommand]
        private void NavigateToRegister_LoginView_Button()
        {
            NavigateToRegisterRequested?.Invoke();
            _logger.LogInformation("Navigating to the registration page.");
        }

        [RelayCommand]
        private void NavigateToForgotPassword_LoginView_Button()
        {
            NavigateToForgotPasswordRequested?.Invoke();
            _logger.LogInformation("Navigating to the forgot password page.");
        }
    }
}