using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FinTrackForWindows.Core;
using FinTrackForWindows.Messages;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class AuthenticatorViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentViewModel;

        private readonly LoginViewModel _loginViewModel;
        private readonly RegisterViewModel _registerViewModel;
        private readonly OtpVerificationViewModel _otpVerificationViewModel;
        private readonly ForgotPasswordViewModel _forgotPasswordViewModel;
        private readonly ApplicationRecognizeSlideViewModel _applicationRecognizeSlideViewModel;

        private readonly ISecureTokenStorage _secureToken;

        private readonly ILogger<AuthenticatorViewModel> _logger;
        private readonly IAppInNotificationService _appInNotificationService;

        public AuthenticatorViewModel(
            LoginViewModel loginViewModel,
            RegisterViewModel registerViewModel,
            OtpVerificationViewModel otpVerificationViewModel,
            ForgotPasswordViewModel forgotPasswordViewModel,
            ApplicationRecognizeSlideViewModel applicationRecognizeSlideViewModel,
            ISecureTokenStorage secureToken,
            ILogger<AuthenticatorViewModel> logger,
            IAppInNotificationService appInNotificationService)
        {
            _loginViewModel = loginViewModel;
            _registerViewModel = registerViewModel;
            _otpVerificationViewModel = otpVerificationViewModel;
            _forgotPasswordViewModel = forgotPasswordViewModel;
            _applicationRecognizeSlideViewModel = applicationRecognizeSlideViewModel;
            _currentViewModel = _applicationRecognizeSlideViewModel;
            _appInNotificationService = appInNotificationService;

            _applicationRecognizeSlideViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;

            _loginViewModel.NavigateToRegisterRequested += () => CurrentViewModel = _registerViewModel;
            _loginViewModel.NavigateToForgotPasswordRequested += () => CurrentViewModel = _forgotPasswordViewModel;

            _registerViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;
            _registerViewModel.NavigateToOtpVerificationRequested += () => CurrentViewModel = _otpVerificationViewModel;

            _otpVerificationViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;
            _otpVerificationViewModel.NavigateToRegisterRequested += () => CurrentViewModel = _registerViewModel;

            _forgotPasswordViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;

            CurrentViewModel = _applicationRecognizeSlideViewModel;

            _registerViewModel.SendOtpVerificationRequested += OnSendOtpVerificationRequested;

            _logger = logger;
            _secureToken = secureToken;

            SavedTokenLogin();
        }

        private void OnSendOtpVerificationRequested(object? sender, EventArgs e)
        {
            _otpVerificationViewModel.StartCounter();
        }

        private void SavedTokenLogin()
        {
            string? token = _secureToken.GetToken();
            if (token is not null)
            {
                bool isValid = TokenValidator.IsTokenValid(token);
                if (!isValid)
                {
                    _appInNotificationService.ShowWarning("Invalid token. Please log in again.");
                    _logger.LogWarning("Invalid token found. User is required to log in again.");
                    SessionManager.ClearToken();
                    _secureToken.ClearToken();
                }
                else
                {
                    SessionManager.SetToken(token);
                    _logger.LogInformation("User already logged in. Token used.");
                    WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
                }
            }
            else
            {
                _logger.LogInformation("User has not logged in yet. Application introduction slides are being shown.");
            }
        }
    }
}