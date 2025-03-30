using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FinTrack.Codes.LoginPanel
{
    public class LoginPanelViewModel : INotifyPropertyChanged
    {
        private LoginPanelView currentView { get; set; }
        private bool showWelcomePanel { get; set; }

        private string inputPassword { get; set; } = "";
        private bool isPasswordVisible = false;

        private string inputEmail { get; set; } = "";

        private string inputName { get; set; } = "";

        #region Commands
        public ICommand ShowLoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public ICommand ShowForgotCommand { get; }
        public ICommand HideWelcomePanelCommand { get; }
        public ICommand ShowCodeVerificationCommand { get; }
        public ICommand ShowResetPasswordCommand { get; }

        public ICommand RegisterCommand { get; }

        public ICommand ShowPasswordInputTextBoxCommand { get; }
        public ICommand ShowPasswordInputPasswordBoxCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        #endregion

        public LoginPanelViewModel()
        {
            currentView = LoginPanelView.Register;
            showWelcomePanel = false;

            ShowLoginCommand = new RelayCommand(o => CurrentView = LoginPanelView.Login);
            ShowRegisterCommand = new RelayCommand(o => CurrentView = LoginPanelView.Register);
            ShowForgotCommand = new RelayCommand(o => CurrentView = LoginPanelView.Forgot);
            HideWelcomePanelCommand = new RelayCommand(o => ShowWelcomePanel = false);
            ShowCodeVerificationCommand = new RelayCommand(o => CurrentView = LoginPanelView.CodeVerification);
            ShowResetPasswordCommand = new RelayCommand(o => CurrentView = LoginPanelView.ResetPassword);

            RegisterCommand = new RelayCommand(o => OnRegister());

            TogglePasswordVisibilityCommand = new RelayCommand(o => TogglePasswordVisibility());
        }

        #region Navigating Between Panels
        public LoginPanelView CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged();
                UpdateVisibility();
            }
        }

        public bool ShowWelcomePanel
        {
            get => showWelcomePanel;
            set
            {
                showWelcomePanel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WelcomeVisibility));
            }
        }

        public Visibility LoginVisibility => CurrentView == LoginPanelView.Login ? Visibility.Visible : Visibility.Hidden;
        public Visibility RegisterVisibility => CurrentView == LoginPanelView.Register ? Visibility.Visible : Visibility.Hidden;
        public Visibility ForgotVisibility => CurrentView == LoginPanelView.Forgot ? Visibility.Visible : Visibility.Hidden;
        public Visibility CodeVerificationVisibility => CurrentView == LoginPanelView.CodeVerification ? Visibility.Visible : Visibility.Hidden;
        public Visibility ResetPasswordVisibility => CurrentView == LoginPanelView.ResetPassword ? Visibility.Visible : Visibility.Hidden;
        public Visibility WelcomeVisibility => ShowWelcomePanel ? Visibility.Visible : Visibility.Hidden;

        private void UpdateVisibility()
        {
            OnPropertyChanged(nameof(LoginVisibility));
            OnPropertyChanged(nameof(RegisterVisibility));
            OnPropertyChanged(nameof(ForgotVisibility));
            OnPropertyChanged(nameof(CodeVerificationVisibility));
            OnPropertyChanged(nameof(ResetPasswordVisibility));
        }

        private void OnRegister()
        {
            ShowWelcomePanel = true;
            CurrentView = LoginPanelView.Login;
        }
        #endregion

        #region User Input Format Control
        public bool IsNameValid(string name) => name.Length >= 5;
        public static bool IsEmailValid(string email) => email.Contains("@") && email.Contains(".");
        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }
            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c))
                {
                    hasUpperCase = true;
                }
                else if (char.IsLower(c))
                {
                    hasLowerCase = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            return hasUpperCase && hasLowerCase && hasDigit;
        }

        public BitmapImage PasswordApprovalIconSource
        {
            get
            {
                string iconPath = IsPasswordValid(InputPassword)
                    ? "pack://application:,,,/Images/Icons/tick.png"
                    : "pack://application:,,,/Images/Icons/x.png";

                return new BitmapImage(new Uri(iconPath));
            }
        }

        public string InputEmail
        {
            get => inputEmail;
            set
            {
                inputEmail = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EmailApprovalIconSource));
            }
        }

        public BitmapImage EmailApprovalIconSource
        {
            get
            {
                string iconPath = IsEmailValid(InputEmail)
                    ? "pack://application:,,,/Images/Icons/tick.png"
                    : "pack://application:,,,/Images/Icons/x.png";

                return new BitmapImage(new Uri(iconPath));
            }
        }

        public string InputUsername
        {
            get => inputName;
            set
            {
                inputName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsernameApprovalIconSource));
            }
        }

        public BitmapImage UsernameApprovalIconSource
        {
            get
            {
                string iconPath = IsNameValid(InputUsername)
                    ? "pack://application:,,,/Images/Icons/tick.png"
                    : "pack://application:,,,/Images/Icons/x.png";

                return new BitmapImage(new Uri(iconPath));
            }
        }
        #endregion

        #region Password Visibility
        public string InputPassword
        {
            get => inputPassword;
            set
            {
                inputPassword = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PasswordApprovalIconSource));
            }
        }

        public bool IsPasswordVisible
        {
            get => isPasswordVisible;
            set
            {
                isPasswordVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EyeIconSource));
            }
        }

        public BitmapImage EyeIconSource
        {
            get
            {
                string uri = IsPasswordVisible 
                    ? "pack://application:,,,/Images/Icons/eyeopen.png" 
                    : "pack://application:,,,/Images/Icons/eyeclose.png";

                return new BitmapImage(new Uri(uri));
            }
        }

        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum LoginPanelView
    {
        Welcome,
        Login,
        Register,
        Forgot,
        CodeVerification,
        ResetPassword
    }
}
