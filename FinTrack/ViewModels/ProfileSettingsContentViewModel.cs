using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;

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

        private readonly IAppInNotificationService _appInNotificationService;

        public ProfileSettingsContentViewModel(ILogger<ProfileSettingsContentViewModel> logger, IApiService apiService, IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _apiService = apiService;
            _appInNotificationService = appInNotificationService;

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
            _logger.LogInformation("New profile information saved: {FullName}, {Email}, {ProfilePhotoUrl}", FullName, Email, ProfilePhotoUrl);
            _appInNotificationService.ShowSuccess("Your profile information has been successfully saved.");
        }
    }
}
