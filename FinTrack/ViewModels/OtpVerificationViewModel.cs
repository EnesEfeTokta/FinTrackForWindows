using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Core;
using FinTrack.Services;
using System.Threading.Tasks;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class OtpVerificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? verificationCode_OtpVerificationView_TextBox;

        [ObservableProperty]
        private string? counterText_OtpVerificationView_TextBlock;

        public event Action? NavigateToLoginRequested;

        public event Action? NavigateToRegisterRequested;

        private readonly AuthService _authService;
        private int _counter;

        public OtpVerificationViewModel()
        {
            _authService = new AuthService();

            if (NewUserInformationManager.Email != null)
            {
                StartCounter();
            }
            else
            {
                MessageBox.Show("Lütfen önce kayıt işlemini tamamlayın.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigateToRegisterRequested?.Invoke();
            }
        }

        private void StartCounter()
        {
            _counter = 300;
            CounterText_OtpVerificationView_TextBlock = $"{_counter / 60}:{_counter % 60:D2}";
            Task.Run(async () =>
            {
                while (_counter > 0)
                {
                    await Task.Delay(1000);
                    _counter--;
                    CounterText_OtpVerificationView_TextBlock = $"{_counter / 60}:{_counter % 60:D2}";
                }
                CounterText_OtpVerificationView_TextBlock = "[X]";
            });
        }

        [RelayCommand]
        private async Task VerifyOtpCode_OtpVerificationView_Button()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode_OtpVerificationView_TextBox) || VerificationCode_OtpVerificationView_TextBox.Length != 6)
            {
                MessageBox.Show("Lütfen geçerli bir OTP kodu girin (6 haneli).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isVerify = await _authService.VerifyOtpAndRegisterCodeAsync(NewUserInformationManager.Email ?? null!, VerificationCode_OtpVerificationView_TextBox ?? null!);
            if (!isVerify)
            {
                MessageBox.Show("OTP doğrulama başarısız oldu. Lütfen kodu kontrol edin ve tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("OTP doğrulama başarılı! Hoş geldiniz!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);

            NewUserInformationManager.FullName = null; // Clear the stored user information
            NewUserInformationManager.Email = null; // Clear the stored user information
            NewUserInformationManager.Password = null; // Clear the stored user information

            NavigateToLoginRequested?.Invoke();
        }

        [RelayCommand]
        private async Task CodeNotFound_OtpVerificationView_Button()
        {
            if (NewUserInformationManager.FullName != null && NewUserInformationManager.Email != null && NewUserInformationManager.Password != null)
            {
                bool isInitiateRegistration = await _authService.InitiateRegistrationAsnc(
                    NewUserInformationManager.FullName,
                    NewUserInformationManager.Email,
                    NewUserInformationManager.Password);

                if (!isInitiateRegistration)
                {
                    MessageBox.Show("Kayıt işlemi başarısız oldu. Lütfen daha sonra tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Doğrulama kodu tekrar gönderildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Kod gönderirken bir sorun oluştu. Bilgileri doğru giriniz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigateToRegisterRequested?.Invoke();
            }
        }
    }
}
