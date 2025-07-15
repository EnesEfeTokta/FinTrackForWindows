using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? email_ForgotPasswordView_TextBox;

        public event Action? NavigateToLoginRequested;

        [RelayCommand]
        private void ResetPassword_ForgotPasswordView_Button()
        {

            if (!string.IsNullOrEmpty(Email_ForgotPasswordView_TextBox))
            {
                MessageBox.Show($"Şifre sıfırlama bağlantısı {Email_ForgotPasswordView_TextBox} adresine gönderildi.");
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi girin.");
            }
        }

        [RelayCommand]
        private void NavigateToLogin_ForgotPasswordView_Button()
        {
            NavigateToLoginRequested?.Invoke();
        }
    }
}
