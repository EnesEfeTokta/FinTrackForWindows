using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class ProfileSettingsContentViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string profilePhotoUrl = string.Empty;

        private readonly ILogger<ProfileSettingsContentViewModel> _logger;

        public ProfileSettingsContentViewModel(ILogger<ProfileSettingsContentViewModel> logger)
        {
            _logger = logger;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            FullName = "John Doe";
            Email = "johndoe@gmail.com";
            ProfilePhotoUrl = "https://example.com/profile-photo.jpg";
        }

        [RelayCommand]
        private void ProfileSettingsContentSaveChanges()
        {
            _logger.LogInformation("Yeni profil bilgileri kaydedildi: {FullName}, {Email}, {ProfilePhotoUrl}", FullName, Email, ProfilePhotoUrl);
            MessageBox.Show("Profil bilgileri başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
