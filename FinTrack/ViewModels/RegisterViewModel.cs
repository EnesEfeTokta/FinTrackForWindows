using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Services;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FinTrackForWindows.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? firstName_RegisterView_TextBox;

        [ObservableProperty]
        private string? lastName_RegisterView_TextBox;

        [ObservableProperty]
        private string? email_RegisterView_TextBox;

        [ObservableProperty]
        private string? password_RegisterView_TextBox;

        [ObservableProperty]
        private bool isPasswordVisible_RegisterView_PasswordBoxAndTextBox = false;

        [ObservableProperty]
        private string eyeIconSource_RegisterView_Image = "/Assets/Images/Icons/eyeclose.png";

        public event Action? NavigateToLoginRequested;
        public event Action? NavigateToOtpVerificationRequested;

        private readonly ILogger<RegisterViewModel> _logger;
        private readonly IAuthService _authService;
        private readonly IAppInNotificationService _appInNotificationService;

        public event EventHandler SendOtpVerificationRequested;

        public RegisterViewModel(ILogger<RegisterViewModel> logger,
            IAuthService authService,
            IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _authService = authService;
            _appInNotificationService = appInNotificationService;
        }

        [RelayCommand]
        private async Task Register_RegisterView_Button()
        {
            NavigateToOtpVerificationRequested?.Invoke();
            if (string.IsNullOrEmpty(FirstName_RegisterView_TextBox) ||
                string.IsNullOrEmpty(LastName_RegisterView_TextBox) ||
                string.IsNullOrEmpty(Email_RegisterView_TextBox) ||
                string.IsNullOrEmpty(Password_RegisterView_TextBox))
            {
                _appInNotificationService.ShowWarning("Please fill in all required fields.");
                _logger.LogWarning("Registration attempt failed: Required fields are missing.");
                return;
            }

            bool isValidEmail = Email_RegisterView_TextBox.Contains("@") && Email_RegisterView_TextBox.Contains(".");
            if (!isValidEmail)
            {
                _appInNotificationService.ShowError("Please enter a valid email address.");
                _logger.LogWarning("Registration attempt failed: Invalid email address provided: {Email}", Email_RegisterView_TextBox);
                return;
            }

            if (!IsPasswordValid(Password_RegisterView_TextBox))
            {
                _appInNotificationService.ShowError("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                _logger.LogWarning("Registration attempt failed: Password does not meet complexity requirements.");
                return;
            }

            bool isInitiateRegistration = await _authService.InitiateRegistrationAsnc(
                FirstName_RegisterView_TextBox,
                LastName_RegisterView_TextBox,
                Email_RegisterView_TextBox,
                Password_RegisterView_TextBox);
            if (!isInitiateRegistration)
            {
                _appInNotificationService.ShowError("Registration failed. Please try again later.");
                _logger.LogError("Registration process failed for email: {Email}", Email_RegisterView_TextBox);
                return;
            }
            SendOtpVerificationRequested?.Invoke(this, EventArgs.Empty);
            _appInNotificationService.ShowSuccess("Registration successful! Please enter the verification code sent to your email address.");
            _logger.LogInformation("Registration successful for email: {Email}", Email_RegisterView_TextBox);

            NewUserInformationManager.FirstName = FirstName_RegisterView_TextBox;
            NewUserInformationManager.LastName = LastName_RegisterView_TextBox;
            NewUserInformationManager.FullName = FirstName_RegisterView_TextBox.Replace(" ", "").Trim() + "_" + LastName_RegisterView_TextBox.Replace(" ", "").Trim();
            NewUserInformationManager.Email = Email_RegisterView_TextBox;
            NewUserInformationManager.Password = Password_RegisterView_TextBox;

            NavigateToOtpVerificationRequested?.Invoke();
        }

        private bool IsPasswordValid(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }

        [RelayCommand]
        private void TogglePasswordVisibility_RegisterView_Button()
        {
            IsPasswordVisible_RegisterView_PasswordBoxAndTextBox = !IsPasswordVisible_RegisterView_PasswordBoxAndTextBox;

            EyeIconSource_RegisterView_Image = IsPasswordVisible_RegisterView_PasswordBoxAndTextBox
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
            _logger.LogInformation("Password visibility toggled. New state: {IsVisible}", IsPasswordVisible_RegisterView_PasswordBoxAndTextBox);
        }

        [RelayCommand]
        private void NavigateToLogin_RegisterView_Button()
        {
            NavigateToLoginRequested?.Invoke();
            _logger.LogInformation("User navigated from registration to login view.");
        }
    }
}