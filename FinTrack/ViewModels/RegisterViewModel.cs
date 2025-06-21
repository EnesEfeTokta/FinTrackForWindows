using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace FinTrack.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        // ... Kayıt için gerekli özellikler (Username, Email, Password vs.)
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _password;

        // "Giriş Yap" ekranına geri dönmek için olay.
        public event Action NavigateToLoginRequested;

        [RelayCommand]
        private void Register()
        {
            // TODO: Kayıt olma mantığı burada işlenecek.
            Console.WriteLine("Register denendi.");
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            // "Giriş ekranına geri dönmek istiyorum" diye haber salıyoruz.
            NavigateToLoginRequested?.Invoke();
        }
    }
}