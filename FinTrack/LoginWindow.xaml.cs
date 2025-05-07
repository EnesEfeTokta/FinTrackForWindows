using FinTrack.Codes.LoginPanel;
using FinTrack.FirstWelcome;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FinTrack
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginPanelViewModel();
            FirstWelcomePanelFrame.Navigate(new WelcomePanel());
        }

        private void FirstWelcomePanelFrame_Navigated(object sender, NavigationEventArgs e) { }
    }
}