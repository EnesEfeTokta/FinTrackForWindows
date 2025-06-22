using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? email_LoginView_TextBox;

        [ObservableProperty]
        private string? password_LoginView_TextBox;

        [ObservableProperty]
        private bool isPasswordVisible_LoginView_PasswordBoxAndTextBox = false;

        [ObservableProperty]
        private string eyeIconSource_LoginView_Image = "/Assets/Images/Icons/eyeclose.png";

        public event Action? NavigateToRegisterRequested;
        public event Action? NavigateToForgotPasswordRequested;

        [RelayCommand]
        private void Login_LoginView_Button()
        {
            MessageBox.Show($"Giriş yapılıyor: Email={Email_LoginView_TextBox}, Password={Password_LoginView_TextBox}");
        }

        [RelayCommand]
        private void NavigateToRegister_LoginView_Button()
        {
            NavigateToRegisterRequested?.Invoke();
        }

        [RelayCommand]
        private void TogglePasswordVisibility_LoginView_Button()
        {
            IsPasswordVisible_LoginView_PasswordBoxAndTextBox = !IsPasswordVisible_LoginView_PasswordBoxAndTextBox;

            EyeIconSource_LoginView_Image = IsPasswordVisible_LoginView_PasswordBoxAndTextBox
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
        }

        [RelayCommand]
        private void NavigateToForgotPassword_LoginView_Button()
        {
            NavigateToForgotPasswordRequested?.Invoke();
        }
    }
}