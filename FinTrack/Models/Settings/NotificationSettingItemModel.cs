using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Enums;

namespace FinTrack.Models.Settings
{
    public partial class NotificationSettingItemModel : ObservableObject
    {
        public NotificationSettingsType SettingType { get; set; }

        [ObservableProperty]
        private bool isEnabled;

        public NotificationSettingItemModel(NotificationSettingsType settingType, bool isEnabled)
        {
            SettingType = settingType;
            this.isEnabled = isEnabled;
        }
    }
}
