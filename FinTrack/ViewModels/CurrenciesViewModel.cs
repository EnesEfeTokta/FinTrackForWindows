using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Currency;
using FinTrackForWindows.Services.Api;
using FinTrackWebApi.Dtos.CurrencyDtos;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.ViewModels
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

        private readonly ILogger<CurrenciesViewModel> _logger;

        private readonly IApiService _apiService;

        public CurrenciesViewModel(ILogger<CurrenciesViewModel> logger, IApiService apiService)
        {
            _logger = logger;

            _apiService = apiService;

            _ = LoadCurrenciesData();
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

        private async Task LoadCurrenciesData()
        {
            var currencies = await _apiService.GetAsync<LatestRatesResponseDto>("Currency/latest/USD"); // TODO: USD yerine kullanıcının seçtiği para birimini kullan
            allCurrencies = new ObservableCollection<CurrencyModel>();
            if (currencies.Rates != null)
            {
                foreach (RateDetailDto item in currencies.Rates)
                {
                    allCurrencies.Add(new CurrencyModel
                    {
                        Id = item.Id,
                        ToCurrencyCode = item.Code,
                        ToCurrencyName = item.CountryCode ?? "N/A",
                        ToCurrencyFlag = item.IconUrl ?? "N/A",
                        ToCurrencyPrice = item.Rate,
                        ToCurrencyChange = "N/A",
                        Type = CurrencyConversionType.Increase, // TODO: Değişim bilgisi eklenmeli
                        DailyLow = "N/A",
                        DailyHigh = "N/A",
                        WeeklyChange = "N/A",
                        MonthlyChange = "N/A"
                    });
                }

                FilterCurrencies();
                SelectedCurrency = FilteredCurrencies.FirstOrDefault();
                _logger.LogInformation("Para birimleri başarıyla yüklendi. Para birimler: {Currencies}", currencies);
            }
        }
    }
}
