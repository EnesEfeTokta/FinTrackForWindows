using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        [ObservableProperty]
        private bool _isPasswordVisible = false;

        [ObservableProperty]
        private string _eyeIconSource = "/Assets/Images/Icons/eyeclose.png";

        public event Action? NavigateToRegisterRequested;
        public event Action? NavigateToForgotPasswordRequested;

        [RelayCommand]
        private async Task Login()
        {
            MessageBox.Show($"Giriş yapılıyor: Email={Email}, Password={Password}");
            await Task.Delay(1000);
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            NavigateToRegisterRequested?.Invoke();
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;

            EyeIconSource = IsPasswordVisible
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
        }

        [RelayCommand]
        private void NavigateToForgotPassword()
        {
            NavigateToForgotPasswordRequested?.Invoke();
        }
    }
}