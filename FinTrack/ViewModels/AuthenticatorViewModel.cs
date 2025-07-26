using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FinTrackForWindows.Core;
using FinTrackForWindows.Messages;
using Microsoft.Extensions.Logging;
using System.Windows;

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

        public AuthenticatorViewModel(
            LoginViewModel loginViewModel,
            RegisterViewModel registerViewModel,
            OtpVerificationViewModel otpVerificationViewModel,
            ForgotPasswordViewModel forgotPasswordViewModel,
            ApplicationRecognizeSlideViewModel applicationRecognizeSlideViewModel,
            ISecureTokenStorage secureToken,
            ILogger<AuthenticatorViewModel> logger)
        {
            _loginViewModel = loginViewModel;
            _registerViewModel = registerViewModel;
            _otpVerificationViewModel = otpVerificationViewModel;
            _forgotPasswordViewModel = forgotPasswordViewModel;
            _applicationRecognizeSlideViewModel = applicationRecognizeSlideViewModel;
            _currentViewModel = _applicationRecognizeSlideViewModel;

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
                    MessageBox.Show("Token geçersiz. Lütfen tekrar giriş yapın.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.LogWarning("Geçersiz token bulundu. Kullanıcıdan yeni giriş yapması istendi.");
                    SessionManager.ClearToken();
                    _secureToken.ClearToken();
                }

                SessionManager.SetToken(token);
                _logger.LogInformation("Kullanıcı zaten giriş yapmış. Token kullanıldı.");
                WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
            }
            else
            {
                _logger.LogInformation("Kullanıcı henüz giriş yapmamış. Uygulama tanıtım slaytları gösteriliyor.");
            }
        }
    }
}