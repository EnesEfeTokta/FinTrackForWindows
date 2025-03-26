using FinTrack.FirstWelcome;
using System.Windows;

namespace FinTrack
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var viewModel = new FirstWelcomeSlideViewModel();
            DataContext = viewModel; // Window’un DataContext’i
            FirstWelcomePanelFrame.Navigate(new WelcomePanel());
        }
    }
}