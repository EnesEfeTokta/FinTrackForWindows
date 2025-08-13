using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Users;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class SecuritySettingsContentViewModel : ObservableObject
    {
        [ObservableProperty] private string currentPassword = string.Empty;
        [ObservableProperty] private string newPassword = string.Empty;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))] private bool _isSaving;

        private readonly ILogger<SecuritySettingsContentViewModel> _logger;
        private readonly IAppInNotificationService _appInNotificationService;
        private readonly IUserStore _userStore;

        public SecuritySettingsContentViewModel(ILogger<SecuritySettingsContentViewModel> logger, IAppInNotificationService appInNotificationService, IUserStore userStore)
        {
            _logger = logger;
            _appInNotificationService = appInNotificationService;
            _userStore = userStore;
        }

        [RelayCommand(CanExecute = nameof(CanSaveChanges))]
        private async Task SaveChanges()
        {
            IsSaving = true;
            var dto = new UpdateUserPasswordDto
            {
                CurrentPassword = this.CurrentPassword,
                NewPassword = this.NewPassword
            };

            bool success = await _userStore.UpdateUserPasswordAsync(dto);
            if (success)
            {
                _logger.LogInformation("Password updated successfully.");
                _appInNotificationService.ShowSuccess("Your password has been changed successfully.");
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
            }
            else
            {
                _logger.LogError("Failed to update password.");
                _appInNotificationService.ShowError("Failed to change your password. Please check your current password.");
            }
            IsSaving = false;
        }

        private bool CanSaveChanges() => !IsSaving;
    }
}