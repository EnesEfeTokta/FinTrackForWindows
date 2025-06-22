using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTrack.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentViewModel;

        private readonly LoginViewModel _loginViewModel;
        private readonly RegisterViewModel _registerViewModel;
        private readonly OtpVerificationViewModel _otpVerificationViewModel;
        private readonly ForgotPasswordViewModel _forgotPasswordViewModel;

        public MainViewModel()
        {
            _loginViewModel = new LoginViewModel();
            _registerViewModel = new RegisterViewModel();
            _otpVerificationViewModel = new OtpVerificationViewModel();
            _forgotPasswordViewModel = new ForgotPasswordViewModel();

            _loginViewModel.NavigateToRegisterRequested += () => CurrentViewModel = _registerViewModel;
            _loginViewModel.NavigateToForgotPasswordRequested += () => CurrentViewModel = _forgotPasswordViewModel;

            _registerViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;

            _otpVerificationViewModel.NavigateToHomeRequested += () => CurrentViewModel = _loginViewModel;

            _forgotPasswordViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;

            CurrentViewModel = _loginViewModel;
        }
    }
}