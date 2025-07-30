using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Services.Api;
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

        private readonly IApiService _apiService;

        public ProfileSettingsContentViewModel(ILogger<ProfileSettingsContentViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
            _ = LoadProfileData();
        }

        private async Task LoadProfileData()
        {
            var profile = await _apiService.GetAsync<ProfileSettingsDto>("UserSettings/ProfileSettings");
            if (profile != null)
            {
                FullName = profile.FullName;
                Email = profile.Email;
                ProfilePhotoUrl = profile.ProfilePictureUrl ?? "N/A";
            }
        }

        [RelayCommand]
        private async Task ProfileSettingsContentSaveChanges()
        {
            await _apiService.PostAsync<object>("UserSettings/ProfileSettings", new ProfileSettingsUpdateDto
            {
                FullName = FullName,
                Email = Email,
                ProfilePictureUrl = ProfilePhotoUrl
            });
            _logger.LogInformation("Yeni profil bilgileri kaydedildi: {FullName}, {Email}, {ProfilePhotoUrl}", FullName, Email, ProfilePhotoUrl);
            MessageBox.Show("Profil bilgileri başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
