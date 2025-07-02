using CommunityToolkit.Mvvm.Messaging;
using FinTrack.Messages;
using FinTrack.ViewModels;
using FinTrack.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Input;

namespace FinTrack
{
    public partial class AuthenticatorWindow : Window, IRecipient<LoginSuccessMessage>
    {
        private readonly IHost _host;

        public AuthenticatorWindow(AuthenticatorViewModel viewModel, IHost host)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            _host = host;

            WeakReferenceMessenger.Default.Register<LoginSuccessMessage>(this);

            this.Unloaded += (s, e) => WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        public void Receive(LoginSuccessMessage message)
        {
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            Application.Current.Dispatcher.Invoke(this.Close);
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