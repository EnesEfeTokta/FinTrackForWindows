using System.Windows;

namespace FinTrackForWindows.Views
{
    public partial class OtpInputDialogWindow : Window
    {
        public string OtpCode { get; private set; } = string.Empty;

        public OtpInputDialogWindow(string newEmail)
        {
            InitializeComponent();
            InfoTextBlock.Text = $"An OTP code has been sent to your current email. Please enter it below to confirm changing your email to {newEmail}.";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            OtpCode = OtpTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
