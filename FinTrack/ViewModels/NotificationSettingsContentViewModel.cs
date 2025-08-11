using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Settings;
using FinTrackForWindows.Services.Users;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinTrackForWindows.ViewModels
{
    public partial class NotificationSettingsContentViewModel : ObservableObject
    {
        public ObservableCollection<NotificationSettingItemModel> EmailNotificationSettings { get; }

        [ObservableProperty] private bool _enableDesktopNotifications;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty, NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))] private bool _isSaving;

        private readonly ILogger<NotificationSettingsContentViewModel> _logger;
        private readonly IUserStore _userStore;

        public NotificationSettingsContentViewModel(ILogger<NotificationSettingsContentViewModel> logger, IUserStore userStore)
        {
            _logger = logger;
            _userStore = userStore;
            EmailNotificationSettings = new ObservableCollection<NotificationSettingItemModel>();

            _userStore.UserChanged += OnUserChanged;
            LoadDataFromStore();
        }

        private void OnUserChanged()
        {
            LoadDataFromStore();
        }

        private void LoadDataFromStore()
        {
            IsLoading = true;
            EmailNotificationSettings.Clear();
            if (_userStore.CurrentUser != null)
            {
                EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.SpendingLimitWarning, _userStore.CurrentUser.SpendingLimitWarning));
                EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.ExpectedBillReminder, _userStore.CurrentUser.ExpectedBillReminder));
                EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.WeeklySpendingSummary, _userStore.CurrentUser.WeeklySpendingSummary));
                EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.NewFeaturesAndAnnouncements, _userStore.CurrentUser.NewFeaturesAndAnnouncements));
                EnableDesktopNotifications = _userStore.CurrentUser.EnableDesktopNotifications;
            }
            IsLoading = false;
        }

        [RelayCommand(CanExecute = nameof(CanSaveChanges))]
        private async Task SaveChanges()
        {
            IsSaving = true;
            var dto = new UserNotificationSettingsUpdateDto
            {
                EnableDesktopNotifications = this.EnableDesktopNotifications,
                SpendingLimitWarning = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.SpendingLimitWarning)?.IsEnabled ?? false,
                ExpectedBillReminder = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.ExpectedBillReminder)?.IsEnabled ?? false,
                WeeklySpendingSummary = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.WeeklySpendingSummary)?.IsEnabled ?? false,
                NewFeaturesAndAnnouncements = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.NewFeaturesAndAnnouncements)?.IsEnabled ?? false
            };

            bool success = await _userStore.UpdateNotificationSettingsAsync(dto);
            if (success)
            {
                _logger.LogInformation("Notification settings saved successfully.");
            }
            else
            {
                _logger.LogError("Failed to save notification settings.");
            }
            IsSaving = false;
        }

        private bool CanSaveChanges() => !IsSaving;
    }
}