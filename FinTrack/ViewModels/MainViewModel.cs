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

        public MainViewModel()
        {
            _loginViewModel = new LoginViewModel();
            _registerViewModel = new RegisterViewModel();
            _otpVerificationViewModel = new OtpVerificationViewModel();

            _loginViewModel.NavigateToRegisterRequested += () => CurrentViewModel = _registerViewModel;
            _registerViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;
            _loginViewModel.NavigateToForgotPasswordRequested += () => CurrentViewModel = _otpVerificationViewModel;

            CurrentViewModel = _loginViewModel;
        }
    }
}