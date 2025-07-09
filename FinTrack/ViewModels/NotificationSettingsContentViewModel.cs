using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using FinTrack.Models.Settings;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class NotificationSettingsContentViewModel : ObservableObject
    {
        public ObservableCollection<NotificationSettingItemModel> EmailNotificationSettings { get; }

        [ObservableProperty]
        private bool enableDesktopNotifications;

        private readonly ILogger<NotificationSettingsContentViewModel> _logger;

        public NotificationSettingsContentViewModel(ILogger<NotificationSettingsContentViewModel> logger)
        {
            _logger = logger;
            EnableDesktopNotifications = true;
            EmailNotificationSettings = new ObservableCollection<NotificationSettingItemModel>();

            LoadSettings();
        }

        private void LoadSettings()
        {
            foreach (NotificationSettingsType settingType in Enum.GetValues(typeof(NotificationSettingsType)))
            {
                bool isInitialSelected = settingType switch
                {
                    NotificationSettingsType.SpendingLimitWarning => true,
                    NotificationSettingsType.ExpectedBillReminder => true,
                    NotificationSettingsType.WeeklySpendingSummary => false,
                    NotificationSettingsType.NewFeaturesAndAnnouncements => false,
                    _ => false
                };
                EmailNotificationSettings.Add(new NotificationSettingItemModel(settingType, isInitialSelected));
            }

            EnableDesktopNotifications = true;
        }

        [RelayCommand]
        private void NotificationSettingsContentChanges()
        {
            var selectedSettings = EmailNotificationSettings
                .Where(setting => setting.IsEnabled)
                .Select(setting => setting.SettingType)
                .ToList();

            _logger.LogInformation("Kullanıcı bildirim ayarlarını değiştirdi: {SelectedSettings}", string.Join(", ", selectedSettings));
            MessageBox.Show(
                "Bildirim ayarlarınız kaydedildi.",
                "Bildirim Ayarları",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
