using CommunityToolkit.Mvvm.ComponentModel;

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

        public AuthenticatorViewModel()
        {
            _loginViewModel = new LoginViewModel();
            _registerViewModel = new RegisterViewModel();
            _otpVerificationViewModel = new OtpVerificationViewModel();
            _forgotPasswordViewModel = new ForgotPasswordViewModel();
            _applicationRecognizeSlideViewModel = new ApplicationRecognizeSlideViewModel();

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