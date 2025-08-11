using FinTrackForWindows.Dtos.CurrencyDtos;
using FinTrackForWindows.Models.Currency;
using FinTrackForWindows.Services.Api;
using FinTrackWebApi.Dtos.CurrencyDtos;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Currencies
{
    public class CurrenciesStore : ICurrenciesStore
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CurrenciesStore> _logger;
        private readonly ObservableCollection<CurrencyModel> _currencies;

        public ReadOnlyObservableCollection<CurrencyModel> Currencies { get; }

        public event NotifyCollectionChangedEventHandler? CurrenciesChanged;

        public CurrenciesStore(IApiService apiService, ILogger<CurrenciesStore> logger)
        {
            _apiService = apiService;
            _logger = logger;

            _currencies = new ObservableCollection<CurrencyModel>();
            Currencies = new ReadOnlyObservableCollection<CurrencyModel>(_currencies);

            _currencies.CollectionChanged += (sender, e) =>
            {
                CurrenciesChanged?.Invoke(this, e);
            };
        }

        public async Task LoadCurrenciesAsync()
        {
            if (_currencies.Any())
            {
                _logger.LogInformation("Currencies are already loaded. Skipping API call.");
                return;
            }

            try
            {
                var response = await _apiService.GetAsync<LatestRatesResponseDto>("Currency/latest/USD");
                if (response?.Rates != null)
                {
                    _currencies.Clear();
                    foreach (var item in response.Rates)
                    {
                        _currencies.Add(new CurrencyModel
                        {
                            Id = item.Id,
                            ToCurrencyCode = item.Code,
                            ToCurrencyName = item.CountryCode ?? "N/A",
                            ToCurrencyFlag = item.IconUrl ?? string.Empty,
                            ToCurrencyPrice = item.Rate
                        });
                    }
                    _logger.LogInformation("{Count} currencies loaded into CurrenciesStore.", _currencies.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the currency list in CurrenciesStore.");
            }
        }

        public async Task<CurrencyHistoryDto?> GetHistoricalDataAsync(string targetCurrencyCode, string period)
        {
            try
            {
                _logger.LogInformation("Historical data for {Target} is requested with {Period} period.", targetCurrencyCode, period);

                string baseCurrency = "USD";
                string endpoint = $"Currency/{baseCurrency}/history/{targetCurrencyCode}?period={period}";
                var historyData = await _apiService.GetAsync<CurrencyHistoryDto>(endpoint);

                return historyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading historical data for {Target} in CurrenciesStore.", targetCurrencyCode);
                return null;
            }
        }

        public async Task<decimal> GetConvertCurrencies(string fromCurrencyCode, string toCurrencyCode, decimal amount)
        {
            try
            {
                _logger.LogInformation("Converting {Amount} from {From} to {To}.", amount, fromCurrencyCode, toCurrencyCode);
                string endpoint = $"Currency/convert?from={fromCurrencyCode}&to={toCurrencyCode}&amount={amount}";
                var response = await _apiService.GetAsync<ConvertResponseDto>(endpoint);
                if (response != null)
                {
                    return response.Result;
                }
                else
                {
                    _logger.LogWarning("A valid response could not be obtained for the conversion operation.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the conversion from {From} to {To} in CurrenciesStore.", fromCurrencyCode, toCurrencyCode);
                return 0;
            }
        }
    }
}