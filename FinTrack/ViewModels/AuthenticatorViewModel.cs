using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace FinTrack.ViewModels
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

        public AuthenticatorViewModel(
            LoginViewModel loginViewModel,
            RegisterViewModel registerViewModel,
            OtpVerificationViewModel otpVerificationViewModel,
            ForgotPasswordViewModel forgotPasswordViewModel,
            ApplicationRecognizeSlideViewModel applicationRecognizeSlideViewModel)
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
        }
    }
}