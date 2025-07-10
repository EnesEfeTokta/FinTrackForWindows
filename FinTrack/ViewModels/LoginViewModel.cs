using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinTrack.Core;
using FinTrack.Messages;
using FinTrack.Services;
using Microsoft.Extensions.Logging;
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

        private readonly IAuthService _authService;
        private readonly ISecureTokenStorage _secureTokenStorage;

        private readonly ILogger<LoginViewModel> _logger;

        public LoginViewModel(
            IAuthService authService,
            ILogger<LoginViewModel> logger,
            ISecureTokenStorage secureTokenStorage)
        {
            _authService = authService;
            _logger = logger;
            _secureTokenStorage = secureTokenStorage;
        }

        [RelayCommand]
        private async Task Login_LoginView_Button()
        {
            _logger.LogInformation("Kullanıcı giriş yapmaya çalışıyor. E-posta: {Email}", Email_LoginView_TextBox);

            if (string.IsNullOrEmpty(Email_LoginView_TextBox) || string.IsNullOrEmpty(Password_LoginView_TextBox))
            {
                MessageBox.Show("Lütfen e-posta ve şifre alanlarını doldurun.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogWarning("Kullanıcı giriş yapmaya çalıştı ancak e-posta veya şifre alanları boş.");
                return;
            }

            string token = await _authService.LoginAsync(Email_LoginView_TextBox, Password_LoginView_TextBox);
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("Giriş başarısız oldu. Lütfen e-posta ve şifrenizi kontrol edin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Kullanıcı giriş yapmaya çalıştı ancak token alınamadı. E-posta veya şifre hatalı olabilir.");
                return;
            }

            SessionManager.SetToken(token);
            _secureTokenStorage.SaveToken(token);
            _logger.LogInformation("Kullanıcı giriş yaptı ve token kaydedildi.");

            WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());
        }

        [RelayCommand]
        private void TogglePasswordVisibility_LoginView_Button()
        {
            IsPasswordVisible_LoginView_PasswordBoxAndTextBox = !IsPasswordVisible_LoginView_PasswordBoxAndTextBox;

            EyeIconSource_LoginView_Image = IsPasswordVisible_LoginView_PasswordBoxAndTextBox
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
            _logger.LogInformation("Şifre görünürlüğü değiştirildi. Şifre {0}.", IsPasswordVisible_LoginView_PasswordBoxAndTextBox ? "görünür" : "gizli");
        }

        [RelayCommand]
        private void NavigateToRegister_LoginView_Button()
        {
            NavigateToRegisterRequested?.Invoke();
            _logger.LogInformation("Kullanıcı kayıt sayfasına yönlendirildi.");
        }

        [RelayCommand]
        private void NavigateToForgotPassword_LoginView_Button()
        {
            NavigateToForgotPasswordRequested?.Invoke();
            _logger.LogInformation("Kullanıcı şifremi unuttum sayfasına yönlendirildi.");
        }
    }
}