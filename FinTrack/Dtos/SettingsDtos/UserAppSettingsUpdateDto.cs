using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Dtos.SettingsDtos
{
    public class UserAppSettingsUpdateDto
    {
        public AppearanceType Appearance { get; set; }
        public BaseCurrencyType Currency { get; set; }
    }
}
