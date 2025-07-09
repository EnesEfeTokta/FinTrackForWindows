using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class SecuritySettingsContentViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        readonly ILogger<SecuritySettingsContentViewModel> _logger;

        public SecuritySettingsContentViewModel(ILogger<SecuritySettingsContentViewModel> logger)
        {
            _logger = logger;
        }

        [RelayCommand]
        private void SecuritySettingsContentSaveChanges()
        {
            _logger.LogInformation("Güvenlik ayarları kaydedildi.");
            MessageBox.Show("Güvenlik ayarları kaydedildi.", "Ayarlar", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
