using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Dialog;
using FinTrackForWindows.Services.Users;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FinTrackForWindows.ViewModels
{
    public partial class ProfileSettingsContentViewModel : ObservableObject
    {
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string profilePhotoUrl = string.Empty;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))] private bool _isSaving;

        private readonly ILogger<ProfileSettingsContentViewModel> _logger;
        private readonly IUserStore _userStore;
        private readonly IDialogService _dialogService;
        private readonly IAppInNotificationService _appInNotificationService;

        public ProfileSettingsContentViewModel(ILogger<ProfileSettingsContentViewModel> logger, IUserStore userStore, IDialogService dialogService, IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _userStore = userStore;
            _dialogService = dialogService;
            _appInNotificationService = appInNotificationService;

            _userStore.UserChanged += OnUserChanged;
            LoadDataFromStore();
        }

        private void OnUserChanged()
        {
            LoadDataFromStore();
        }

        private void LoadDataFromStore()
        {
            if (_userStore.CurrentUser != null)
            {
                var nameParts = _userStore.CurrentUser.UserName.Split('_');
                FirstName = nameParts.FirstOrDefault() ?? string.Empty;
                LastName = nameParts.Length > 1 ? nameParts.Last() : string.Empty;
                Email = _userStore.CurrentUser.Email;
                ProfilePhotoUrl = _userStore.CurrentUser.ProfilePictureUrl ?? string.Empty;
            }
        }

        [RelayCommand]
        private async Task RequestEmailChange()
        {
            string newEmail = this.Email;
            if (newEmail == _userStore.CurrentUser?.Email)
            {
                _appInNotificationService.ShowWarning("The new email address cannot be the same as the current one.");
                return;
            }

            await _userStore.RequestEmailChangeOtpAsync();
            var otp = _dialogService.ShowOtpDialog(newEmail);

            if (!string.IsNullOrEmpty(otp))
            {
                var dto = new UpdateUserEmailDto { NewEmail = newEmail, OtpCode = otp };
                bool success = await _userStore.UpdateUserEmailAsync(dto);
                if (success)
                {
                    _appInNotificationService.ShowSuccess("Your email has been changed successfully.");
                }
                else
                {
                    _appInNotificationService.ShowError("Failed to change your email. The OTP may be incorrect or expired.");
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveChanges))]
        private async Task SaveChanges()
        {
            IsSaving = true;

            var nameDto = new UpdateUserNameDto { FirstName = this.FirstName, LastName = this.LastName };
            var picDto = new UpdateProfilePictureDto { ProfilePictureUrl = this.ProfilePhotoUrl };

            bool nameSuccess = await _userStore.UpdateUserNameAsync(nameDto);
            bool picSuccess = await _userStore.UpdateProfilePictureAsync(picDto);

            if (nameSuccess && picSuccess)
            {
                _logger.LogInformation("Profile settings saved successfully.");
                _appInNotificationService.ShowSuccess("Your profile has been updated.");
            }
            else
            {
                _logger.LogError("Failed to save one or more profile settings.");
                _appInNotificationService.ShowError("There was an issue saving your profile.");
            }
            IsSaving = false;
        }

        private bool CanSaveChanges() => !IsSaving;
    }
}