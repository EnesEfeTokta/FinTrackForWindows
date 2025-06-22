using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTrack.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentViewModel;

        private readonly LoginViewModel _loginViewModel;
        private readonly RegisterViewModel _registerViewModel;

        public MainViewModel()
        {
            _loginViewModel = new LoginViewModel();
            _registerViewModel = new RegisterViewModel();

            _loginViewModel.NavigateToRegisterRequested += () => CurrentViewModel = _registerViewModel;
            _registerViewModel.NavigateToLoginRequested += () => CurrentViewModel = _loginViewModel;

            CurrentViewModel = _loginViewModel;
        }
    }
}