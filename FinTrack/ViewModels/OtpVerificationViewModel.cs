using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Services;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class OtpVerificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? verificationCode_OtpVerificationView_TextBox;

        [ObservableProperty]
        private string? counterText_OtpVerificationView_TextBlock;

        public event Action? NavigateToLoginRequested;

        public event Action? NavigateToRegisterRequested;

        private int _counter;
        private Task? _counterTask;

        private readonly IAuthService _authService;
        private readonly ILogger<OtpVerificationViewModel> _logger;
        private readonly IAppInNotificationService _appInNotificationService;

        public OtpVerificationViewModel(IAuthService authService, ILogger<OtpVerificationViewModel> logger, IAppInNotificationService appInNotificationService)
        {
            _authService = authService;
            _logger = logger;
            _appInNotificationService = appInNotificationService;
        }

        public void StartCounter()
        {
            _counter = 300;
            CounterText_OtpVerificationView_TextBlock = $"{_counter / 60}:{_counter % 60:D2}";
            Task.Run(async () =>
            {
                while (_counter > 0)
                {
                    await Task.Delay(1000);
                    _counter--;
                    CounterText_OtpVerificationView_TextBlock = $"{_counter / 60}:{_counter % 60:D2}";
                }
                CounterText_OtpVerificationView_TextBlock = "[X]";
            });
        }

        [RelayCommand]
        private async Task VerifyOtpCode_OtpVerificationView_Button()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode_OtpVerificationView_TextBox) || VerificationCode_OtpVerificationView_TextBox.Length != 6)
            {
                _appInNotificationService.ShowError("Please enter a valid 6-digit OTP code.");
                _logger.LogWarning("Invalid OTP code entry. Prompted user to try again.");
                return;
            }

            bool isVerify = await _authService.VerifyOtpAndRegisterCodeAsync(NewUserInformationManager.Email ?? null!, VerificationCode_OtpVerificationView_TextBox ?? null!);
            if (!isVerify)
            {
                _appInNotificationService.ShowError("OTP verification failed. Please check the code and try again.");
                _logger.LogError("OTP verification failed. Email: {Email}, Code: {Code}", NewUserInformationManager.Email, VerificationCode_OtpVerificationView_TextBox);
                return;
            }
            _appInNotificationService.ShowSuccess("OTP verification successful! Welcome!");
            _logger.LogInformation("OTP verification successful. User registration completed. Email: {Email}", NewUserInformationManager.Email);

            NewUserInformationManager.FullName = null;
            NewUserInformationManager.Email = null;
            NewUserInformationManager.Password = null;

            NavigateToLoginRequested?.Invoke();
        }

        [RelayCommand]
        private async Task CodeNotFound_OtpVerificationView_Button()
        {
            if (string.IsNullOrEmpty(NewUserInformationManager.FirstName) ||
                string.IsNullOrEmpty(NewUserInformationManager.LastName) ||
                string.IsNullOrEmpty(NewUserInformationManager.Email) ||
                string.IsNullOrEmpty(NewUserInformationManager.Password))
            {
                bool isInitiateRegistration = await _authService.InitiateRegistrationAsnc(
                    NewUserInformationManager.FirstName ?? string.Empty,
                    NewUserInformationManager.LastName ?? string.Empty,
                    NewUserInformationManager.Email ?? string.Empty,
                    NewUserInformationManager.Password ?? string.Empty);

                if (!isInitiateRegistration)
                {
                    _appInNotificationService.ShowError("Registration failed. Please try again later.");
                    _logger.LogError("Registration failed. Email: {Email}", NewUserInformationManager.Email);
                    return;
                }
                _appInNotificationService.ShowInfo("Verification code has been resent. Please check your email.");
                _logger.LogInformation("Verification code resent. Email: {Email}", NewUserInformationManager.Email);
            }
            else
            {
                _appInNotificationService.ShowError("An issue occurred while sending the code. Please check your information.");
                _logger.LogWarning("Missing user information during code sending. Redirecting user to registration page.");
                NavigateToRegisterRequested?.Invoke();
            }
        }

        [RelayCommand]
        private void NavigateToRegister_OtpVerificationView_Button()
        {
            NavigateToRegisterRequested?.Invoke();
            _logger.LogInformation("User redirected to the registration page.");
        }
    }
}
