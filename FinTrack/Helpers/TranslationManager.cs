using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

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
            set
            {
                if (_currentLanguage != value)
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        public string GetString(string key)
        {
            string? result = _resourceManager.GetString(key, _currentLanguage);
            return result ?? $"_{key}_";
        }

        public string this[string key] => GetString(key);
    }
}
