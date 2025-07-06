using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Models.Currency;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrack.ViewModels
{
    public partial class CurrenciesViewModel : ObservableObject
    {
        private ObservableCollection<CurrencyModel> allCurrencies;

        [ObservableProperty]
        private ObservableCollection<CurrencyModel> filteredCurrencies;

        [ObservableProperty]
        private CurrencyModel? selectedCurrency;

        [ObservableProperty]
        private string currencySearch = string.Empty;

        [ObservableProperty]
        private string toCurrencyCode = string.Empty;

        [ObservableProperty]
        private string toCurrencyName = string.Empty;

        [ObservableProperty]
        private string toCurrencyPrice = string.Empty;

        [ObservableProperty]
        private string weeklyChange = string.Empty;

        [ObservableProperty]
        private string monthlyChange = string.Empty;

        private readonly ILogger<CurrenciesViewModel> _logger;

        public CurrenciesViewModel(ILogger<CurrenciesViewModel> logger)
        {
            _logger = logger;

            LoadSampleData();
        }
        partial void OnCurrencySearchChanged(string value)
        {
            FilterCurrencies();
        }

        private void FilterCurrencies()
        {
            if (string.IsNullOrWhiteSpace(CurrencySearch))
            {
                FilteredCurrencies = new ObservableCollection<CurrencyModel>(allCurrencies);
            }
            else
            {
                var filtered = allCurrencies
                    .Where(c => c.ToCurrencyCode.Contains(CurrencySearch, StringComparison.OrdinalIgnoreCase) ||
                                c.ToCurrencyName.Contains(CurrencySearch, StringComparison.OrdinalIgnoreCase));
                FilteredCurrencies = new ObservableCollection<CurrencyModel>(filtered);
            }

            _logger.LogInformation("Para birimleri '{SearchText}' metnine göre filtrelendi.", CurrencySearch);
        }

        private void LoadSampleData()
        {
            allCurrencies = new ObservableCollection<CurrencyModel>
            {
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/try.png",
                    ToCurrencyCode = "TRY",
                    ToCurrencyName = "Turkish Lira",
                    ToCurrencyPrice = 32.56m,
                    ToCurrencyChange = "+0.12 (0.37%)",
                    Type = Enums.CurrencyConversionType.Increase
                },
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/eur.png",
                    ToCurrencyCode = "EUR",
                    ToCurrencyName = "Euro",
                    ToCurrencyPrice = 3.06m,
                    ToCurrencyChange = "-0.12 (0.37%)",
                    Type = Enums.CurrencyConversionType.Decrease
                },
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/gbp.png",
                    ToCurrencyCode = "GBP",
                    ToCurrencyName = "British Pound",
                    ToCurrencyPrice = 0.08m,
                    ToCurrencyChange = "+0.20 (0.80%)",
                    Type = Enums.CurrencyConversionType.Decrease
                },
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/eur.png",
                    ToCurrencyCode = "EUR",
                    ToCurrencyName = "Euro",
                    ToCurrencyPrice = 3.06m,
                    ToCurrencyChange = "-0.12 (0.37%)",
                    Type = Enums.CurrencyConversionType.Decrease
                },
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/gbp.png",
                    ToCurrencyCode = "GBP",
                    ToCurrencyName = "British Pound",
                    ToCurrencyPrice = 0.08m,
                    ToCurrencyChange = "+0.20 (0.80%)",
                    Type = Enums.CurrencyConversionType.Decrease
                },
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/eur.png",
                    ToCurrencyCode = "EUR",
                    ToCurrencyName = "Euro",
                    ToCurrencyPrice = 3.06m,
                    ToCurrencyChange = "-0.12 (0.37%)",
                    Type = Enums.CurrencyConversionType.Decrease
                },
                new CurrencyModel
                {
                    ToCurrencyFlag = "https://currencyfreaks.com/photos/flags/gbp.png",
                    ToCurrencyCode = "GBP",
                    ToCurrencyName = "British Pound",
                    ToCurrencyPrice = 0.08m,
                    ToCurrencyChange = "+0.20 (0.80%)",
                    Type = Enums.CurrencyConversionType.Decrease
                }
            };
            FilterCurrencies();
            SelectedCurrency = FilteredCurrencies.FirstOrDefault();
            _logger.LogInformation("Örnek para birimleri yüklendi.");
        }
    }
}
