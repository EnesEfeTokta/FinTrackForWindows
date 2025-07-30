using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Settings;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class NotificationSettingsContentViewModel : ObservableObject
    {
        public ObservableCollection<NotificationSettingItemModel> EmailNotificationSettings { get; }

        [ObservableProperty]
        private bool _enableDesktopNotifications;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
        private bool _isSaving;

        private readonly ILogger<NotificationSettingsContentViewModel> _logger;
        private readonly IApiService _apiService;

        public NotificationSettingsContentViewModel(ILogger<NotificationSettingsContentViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            EmailNotificationSettings = new ObservableCollection<NotificationSettingItemModel>();
            _ = LoadSettings();
        }

        private async Task LoadSettings()
        {
            IsLoading = true;
            try
            {
                var settingsDto = await _apiService.GetAsync<UserNotificationSettingsDto>("UserSettings/UserNotificationSettings");
                EmailNotificationSettings.Clear();

                if (settingsDto != null)
                {
                    // DTO'dan Model Listesine Mapping
                    EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.SpendingLimitWarning, settingsDto.SpendingLimitWarning));
                    EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.ExpectedBillReminder, settingsDto.ExpectedBillReminder));
                    EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.WeeklySpendingSummary, settingsDto.WeeklySpendingSummary));
                    EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.NewFeaturesAndAnnouncements, settingsDto.NewFeaturesAndAnnouncements));
                    EnableDesktopNotifications = settingsDto.EnableDesktopNotifications;
                }
                else
                {
                    // API'den veri gelmezse varsayılan değerlerle doldur
                    InitializeDefaultSettings();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notification settings.");
                MessageBox.Show("Could not load notification settings. Default values will be used.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                InitializeDefaultSettings();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveChanges))]
        private async Task SaveChanges()
        {
            IsSaving = true;
            try
            {
                var settingsUpdateDto = new UserNotificationSettingsUpdateDto
                {
                    EnableDesktopNotifications = this.EnableDesktopNotifications,
                    SpendingLimitWarning = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.SpendingLimitWarning)?.IsEnabled ?? false,
                    ExpectedBillReminder = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.ExpectedBillReminder)?.IsEnabled ?? false,
                    WeeklySpendingSummary = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.WeeklySpendingSummary)?.IsEnabled ?? false,
                    NewFeaturesAndAnnouncements = EmailNotificationSettings.FirstOrDefault(s => s.SettingType == NotificationSettingsType.NewFeaturesAndAnnouncements)?.IsEnabled ?? false
                };

                await _apiService.PostAsync<object>("UserSettings/UserNotificationSettings", settingsUpdateDto);

                _logger.LogInformation("User notification settings have been successfully saved.");
                MessageBox.Show("Your notification settings have been saved.", "Notification Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save notification settings.");
                MessageBox.Show("An error occurred while saving your settings. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSaveChanges() => !IsSaving;

        private void InitializeDefaultSettings()
        {
            EmailNotificationSettings.Clear();
            EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.SpendingLimitWarning, true));
            EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.ExpectedBillReminder, true));
            EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.WeeklySpendingSummary, false));
            EmailNotificationSettings.Add(new NotificationSettingItemModel(NotificationSettingsType.NewFeaturesAndAnnouncements, false));
            EnableDesktopNotifications = true;
        }
    }
}