namespace FinTrackForWindows.Services.ApplySettings
{
    public class ApplySettingsService : IApplySettingsService
    {
        public event Action? SettingsChanged;

        public void BaseCurrencyApply()
        {
            SettingsChanged?.Invoke();
        }

        public void AppearanceApply()
        {
            SettingsChanged?.Invoke();
        }

        public void LanguageApply()
        {
            SettingsChanged?.Invoke();
        }
    }
}
