using FinTrackForWindows.Enums;
using FinTrackForWindows.Services.ApplySettings;
using FinTrackForWindows.Services.Users;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace FinTrackForWindows.Helpers
{
    public class TranslationManager : INotifyPropertyChanged
    {
        private static readonly TranslationManager _instance = new TranslationManager();
        public static TranslationManager Instance => _instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ResourceManager _resourceManager = new ResourceManager("FinTrackForWindows.Localization.Strings", typeof(TranslationManager).Assembly);

        private CultureInfo _currentLanguage = CultureInfo.CurrentUICulture;

        public CultureInfo CurrentLanguage
        {
            get => _currentLanguage;
            private set
            {
                if (_currentLanguage.Name != value.Name)
                {
                    _currentLanguage = value;
                    Thread.CurrentThread.CurrentUICulture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        private static IUserStore? _userStore;

        public static void Initialize(IApplySettingsService settingsService, IUserStore userStore)
        {
            _userStore = userStore;
            settingsService.SettingsChanged += OnSettingsChanged;

            UpdateLanguageFromStore();
        }

        private static void OnSettingsChanged()
        {
            UpdateLanguageFromStore();
        }

        private static void UpdateLanguageFromStore()
        {
            if (_userStore?.CurrentUser == null) return;

            var languageType = _userStore.CurrentUser.Language;
            CultureInfo newCulture;

            switch (languageType)
            {
                case LanguageType.tr_TR:
                    newCulture = new CultureInfo("tr-TR");
                    break;
                case LanguageType.de_DE:
                    newCulture = new CultureInfo("de-DE");
                    break;
                case LanguageType.fr_FR:
                    newCulture = new CultureInfo("fr-FR");
                    break;
                case LanguageType.es_ES:
                    newCulture = new CultureInfo("es-ES");
                    break;
                case LanguageType.it_IT:
                    newCulture = new CultureInfo("it-IT");
                    break;
                case LanguageType.en_US:
                    newCulture = new CultureInfo("en-US");
                    break;
                default:
                    newCulture = new CultureInfo("en-US");
                    break;
            }

            Instance.CurrentLanguage = newCulture;
        }

        public string GetString(string key)
        {
            string? result = _resourceManager.GetString(key, _currentLanguage);
            return result ?? $"_{key}_";
        }

        public string this[string key] => GetString(key);
    }
}
