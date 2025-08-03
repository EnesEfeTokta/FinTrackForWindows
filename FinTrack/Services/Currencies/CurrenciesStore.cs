using FinTrackForWindows.Dtos.CurrencyDtos;
using FinTrackForWindows.Models.Currency;
using FinTrackForWindows.Services.Api;
using FinTrackWebApi.Dtos.CurrencyDtos;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.Services.Currencies
{
    public class CurrenciesStore : ICurrenciesStore
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CurrenciesStore> _logger;
        private readonly ObservableCollection<CurrencyModel> _currencies;

        public ReadOnlyObservableCollection<CurrencyModel> Currencies { get; }

        public CurrenciesStore(IApiService apiService, ILogger<CurrenciesStore> logger)
        {
            _apiService = apiService;
            _logger = logger;

            _currencies = new ObservableCollection<CurrencyModel>();
            Currencies = new ReadOnlyObservableCollection<CurrencyModel>(_currencies);
        }

        public async Task LoadCurrenciesAsync()
        {
            if (_currencies.Any())
            {
                _logger.LogInformation("Para birimleri zaten yüklü. API çağrısı atlanıyor.");
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
                    _logger.LogInformation("{Count} adet para birimi CurrenciesStore'a yüklendi.", _currencies.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrenciesStore'da para birimi listesi yüklenirken hata oluştu.");
            }
        }

        public async Task<CurrencyHistoryDto?> GetHistoricalDataAsync(string targetCurrencyCode, string period)
        {
            try
            {
                _logger.LogInformation("{Target} için geçmiş veriler {Period} periyoduyla isteniyor.", targetCurrencyCode, period);

                string baseCurrency = "USD";
                string endpoint = $"Currency/{baseCurrency}/history/{targetCurrencyCode}?period={period}";
                var historyData = await _apiService.GetAsync<CurrencyHistoryDto>(endpoint);

                return historyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Target} için geçmiş veriler yüklenirken CurrenciesStore'da hata oluştu.", targetCurrencyCode);
                return null;
            }
        }

        public async Task<decimal> GetConvertCurrencies(string fromCurrencyCode, string toCurrencyCode, decimal amount)
        {
            try
            {
                _logger.LogInformation("{From} para biriminden {To} para birimine {Amount} miktarı dönüştürülüyor.", fromCurrencyCode, toCurrencyCode, amount);
                string endpoint = $"Currency/convert?from={fromCurrencyCode}&to={toCurrencyCode}&amount={amount}";
                var response = await _apiService.GetAsync<ConvertResponseDto>(endpoint);
                if (response != null)
                {
                    return response.Result;
                }
                else
                {
                    _logger.LogWarning("Dönüştürme işlemi için geçerli bir yanıt alınamadı.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{From} para biriminden {To} para birimine dönüştürme işlemi sırasında hata oluştu.", fromCurrencyCode, toCurrencyCode);
                return 0;
            }
        }
    }
}