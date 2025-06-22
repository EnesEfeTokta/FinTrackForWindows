using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

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

        [RelayCommand]
        private void Register_RegisterView_Button()
        {
            Console.WriteLine("Register denendi.");
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