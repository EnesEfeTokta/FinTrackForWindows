using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class AppSettingsContentViewModel : ObservableObject
    {
        public ObservableCollection<AppearanceType> AppearanceTypes { get; }

        public ObservableCollection<BaseCurrencyType> CurrencyTypes { get; }

        [ObservableProperty]
        private bool isFirstOpening = true;

        private readonly ILogger<AppSettingsContentViewModel> _logger;

        public AppSettingsContentViewModel(ILogger<AppSettingsContentViewModel> logger)
        {
            _logger = logger;
            AppearanceTypes = new ObservableCollection<AppearanceType>();
            CurrencyTypes = new ObservableCollection<BaseCurrencyType>();

            InitializeAppearanceTypes();
        }

        private void InitializeAppearanceTypes()
        {
            AppearanceTypes.Clear();
            foreach (AppearanceType appearanceType in Enum.GetValues(typeof(AppearanceType)))
            {
                AppearanceTypes.Add(appearanceType);
            }

            CurrencyTypes.Clear();
            foreach (BaseCurrencyType currencyType in Enum.GetValues(typeof(BaseCurrencyType)))
            {
                CurrencyTypes.Add(currencyType);
            }
        }

        [RelayCommand]
        private void AppSettingsContentChanges()
        {
            _logger.LogInformation("Kullanıcı uygulama ayarlarını değiştirdi. İlk açılış: {IsFirstOpening}", IsFirstOpening);
            MessageBox.Show("Kullanıcı uygulama ayarlarını değiştirdi.",
                            "Uygulama Ayarları",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}