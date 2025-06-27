using FinTrack.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace FinTrack
{
    public partial class AuthenticatorWindow : Window
    {
        public AuthenticatorWindow()
        {
            InitializeComponent();
            this.DataContext = new AuthenticatorViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}