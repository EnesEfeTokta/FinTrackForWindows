using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Services.AppInNotifications;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class SecuritySettingsContentViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        readonly ILogger<SecuritySettingsContentViewModel> _logger;

        private readonly IAppInNotificationService _appInNotificationService;

        public SecuritySettingsContentViewModel(ILogger<SecuritySettingsContentViewModel> logger, IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _appInNotificationService = appInNotificationService;
        }

        [RelayCommand]
        private void SecuritySettingsContentSaveChanges()
        {
            _logger.LogInformation("Security settings saved.");
            _appInNotificationService.ShowSuccess("Security settings saved.");
        }
    }
}
