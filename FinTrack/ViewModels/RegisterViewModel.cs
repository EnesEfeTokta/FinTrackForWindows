using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Core;
using FinTrack.Services;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? fullName_RegisterView_TextBox;

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

        private readonly AuthService _authService;

        public RegisterViewModel()
        {
            _authService = new AuthService();
        }

        [RelayCommand]
        private async Task Register_RegisterView_Button()
        {
            NavigateToOtpVerificationRequested?.Invoke();
            if (string.IsNullOrEmpty(FullName_RegisterView_TextBox) ||
                string.IsNullOrEmpty(Email_RegisterView_TextBox) ||
                string.IsNullOrEmpty(Password_RegisterView_TextBox))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isValidEmail = Email_RegisterView_TextBox.Contains("@") && Email_RegisterView_TextBox.Contains(".");
            if (!isValidEmail)
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isInitiateRegistration = await _authService.InitiateRegistrationAsnc(
                FullName_RegisterView_TextBox,
                Email_RegisterView_TextBox,
                Password_RegisterView_TextBox);
            if (!isInitiateRegistration)
            {
                MessageBox.Show("Kayıt işlemi başarısız oldu. Lütfen daha sonra tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Kayıt işlemi başarılı! Lütfen e-posta adresinize gelen doğrulama kodunu girin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);

            // Store user information in the static manager
            NewUserInformationManager.FullName = FullName_RegisterView_TextBox;
            NewUserInformationManager.Email = Email_RegisterView_TextBox;
            NewUserInformationManager.Password = Password_RegisterView_TextBox;

            NavigateToOtpVerificationRequested?.Invoke();
        }

        [RelayCommand]
        private void TogglePasswordVisibility_RegisterView_Button()
        {
            IsPasswordVisible_RegisterView_PasswordBoxAndTextBox = !IsPasswordVisible_RegisterView_PasswordBoxAndTextBox;

            EyeIconSource_RegisterView_Image = IsPasswordVisible_RegisterView_PasswordBoxAndTextBox
                ? "/Assets/Images/Icons/eyeopen.png"
                : "/Assets/Images/Icons/eyeclose.png";
        }

        [RelayCommand]
        private void NavigateToLogin_RegisterView_Button()
        {
            NavigateToLoginRequested?.Invoke();
        }
    }
}