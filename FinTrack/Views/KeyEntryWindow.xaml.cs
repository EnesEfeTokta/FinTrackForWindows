using System.Windows;

namespace FinTrackForWindows.Views
{
    public partial class KeyEntryWindow : Window
    {
        public string EnteredKey { get; private set; } = string.Empty;

        public KeyEntryWindow()
        {
            InitializeComponent();

            KeyTextBox.Focus();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(KeyTextBox.Text))
            {
                EnteredKey = KeyTextBox.Text.Trim();
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Encryption key cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
