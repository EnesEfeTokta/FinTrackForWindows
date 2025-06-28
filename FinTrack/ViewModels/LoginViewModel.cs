using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinTrack.Core;
using FinTrack.Messages;
using FinTrack.Services;
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

        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService();
            SavedTokenLogin();
        }

        private void SavedTokenLogin()
        {
            SecureTokenStorage secureTokenStorage = new SecureTokenStorage();
            string? token = secureTokenStorage.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                SessionManager.SetToken(token);
                MessageBox.Show("Giriş başarılı! Token kullanıldı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
            }
        }

        [RelayCommand]
        private async Task Login_LoginView_Button()
        {
            WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
            if (string.IsNullOrEmpty(Email_LoginView_TextBox) || string.IsNullOrEmpty(Password_LoginView_TextBox))
            {
                MessageBox.Show("Lütfen e-posta ve şifre alanlarını doldurun.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string token = await _authService.LoginAsync(Email_LoginView_TextBox, Password_LoginView_TextBox);
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("Giriş başarısız oldu. Lütfen e-posta ve şifrenizi kontrol edin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SessionManager.SetToken(token);

            SecureTokenStorage secureTokenStorage = new SecureTokenStorage();
            secureTokenStorage.SaveToken(token);

            MessageBox.Show("Giriş başarılı!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);

            WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
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
        private void NavigateToRegister_LoginView_Button()
        {
            NavigateToRegisterRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToForgotPassword_LoginView_Button()
        {
            NavigateToForgotPasswordRequested?.Invoke();
        }
    }
}