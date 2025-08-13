namespace FinTrackForWindows.Services.ApplySettings
{
    public interface IApplySettingsService
    {
        event Action? SettingsChanged;

        void BaseCurrencyApply();
        void AppearanceApply();
        void LanguageApply();
    }
}
