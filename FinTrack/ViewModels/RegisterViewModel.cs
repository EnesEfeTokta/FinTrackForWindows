using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Services;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? firstName_RegisterView_TextBox;

        [ObservableProperty]
        private string? lastName_RegisterView_TextBox;

        [ObservableProperty]
        private string? email_RegisterView_TextBox;

        [ObservableProperty]
        private string? password_RegisterView_TextBox;

        [ObservableProperty]
        private bool isPasswordVisible_RegisterView_PasswordBoxAndTextBox = false;

        [ObservableProperty]
        private string eyeIconSource_RegisterView_Image = "/Assets/Images/Icons/eyeclose.png";

        public event Action? NavigateToLoginRequested;
        public event Action? NavigateToOtpVerificationRequested;

        private readonly ILogger<RegisterViewModel> _logger;
        private readonly IAuthService _authService;

        public event EventHandler SendOtpVerificationRequested;

        public RegisterViewModel(ILogger<RegisterViewModel> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [RelayCommand]
        private async Task Register_RegisterView_Button()
        {
            NavigateToOtpVerificationRequested?.Invoke();
            if (string.IsNullOrEmpty(FirstName_RegisterView_TextBox) ||
                string.IsNullOrEmpty(LastName_RegisterView_TextBox) ||
                string.IsNullOrEmpty(Email_RegisterView_TextBox) ||
                string.IsNullOrEmpty(Password_RegisterView_TextBox))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogWarning("Kayıt işlemi için gerekli alanlar boş bırakıldı.");
                return;
            }

            bool isValidEmail = Email_RegisterView_TextBox.Contains("@") && Email_RegisterView_TextBox.Contains(".");
            if (!isValidEmail)
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogWarning("Kayıt işlemi için geçersiz e-posta adresi girildi: {Email}", Email_RegisterView_TextBox);
                return;
            }

            if (IsPasswordValid(Password_RegisterView_TextBox) == false)
            {
                MessageBox.Show("Şifre en az 8 karakter uzunluğunda, en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogWarning("Kayıt işlemi için geçersiz şifre girildi.");
                return;
            }

            bool isInitiateRegistration = await _authService.InitiateRegistrationAsnc(
                FirstName_RegisterView_TextBox,
                LastName_RegisterView_TextBox,
                Email_RegisterView_TextBox,
                Password_RegisterView_TextBox);
            if (!isInitiateRegistration)
            {
                MessageBox.Show("Kayıt işlemi başarısız oldu. Lütfen daha sonra tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Kayıt işlemi başarısız oldu. E-posta: {Email}", Email_RegisterView_TextBox);
                return;
            }
            SendOtpVerificationRequested?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Kayıt işlemi başarılı! Lütfen e-posta adresinize gelen doğrulama kodunu girin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            _logger.LogInformation("Kayıt işlemi başarılı. E-posta: {Email}", Email_RegisterView_TextBox);

            // Store user information in the static manager
            NewUserInformationManager.FirstName = FirstName_RegisterView_TextBox;
            NewUserInformationManager.LastName = LastName_RegisterView_TextBox;
            NewUserInformationManager.FullName = FirstName_RegisterView_TextBox.Replace(" ", "").Trim() + "_" + LastName_RegisterView_TextBox.Replace(" ", "").Trim();
            NewUserInformationManager.Email = Email_RegisterView_TextBox;
            NewUserInformationManager.Password = Password_RegisterView_TextBox;

            NavigateToOtpVerificationRequested?.Invoke();
        }

        private bool IsPasswordValid(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }

        [RelayCommand]
        private void TogglePasswordVisibility_RegisterView_Button()
        {
            IsPasswordVisible_RegisterView_PasswordBoxAndTextBox = !IsPasswordVisible_RegisterView_PasswordBoxAndTextBox;

            EyeIconSource_RegisterView_Image = IsPasswordVisible_RegisterView_PasswordBoxAndTextBox
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
            _logger.LogInformation("Şifre görünürlüğü değiştirildi. Yeni durum: {IsVisible}", IsPasswordVisible_RegisterView_PasswordBoxAndTextBox);
        }

        [RelayCommand]
        private void NavigateToLogin_RegisterView_Button()
        {
            NavigateToLoginRequested?.Invoke();
            _logger.LogInformation("Kullanıcı kayıt ekranından giriş ekranına yönlendirildi.");
        }
    }
}