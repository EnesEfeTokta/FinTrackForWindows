using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class OtpVerificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _otpCode;

        [ObservableProperty]
        private bool _isOtpCodeValid = true;

        public event Action? NavigateToHomeRequested;

        public event Action? NavigateToLoginRequested;

        [RelayCommand]
        private void VerifyOtp()
        {
            if (string.IsNullOrWhiteSpace(OtpCode) || OtpCode.Length != 6)
            {
                IsOtpCodeValid = false;
                MessageBox.Show("Lütfen geçerli bir OTP kodu girin.");
                return;
            }

            IsOtpCodeValid = true;
            MessageBox.Show("OTP doğrulandı. Ana sayfaya yönlendiriliyor...");
            NavigateToHomeRequested?.Invoke();
        }
    }
}
