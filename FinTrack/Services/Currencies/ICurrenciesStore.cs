using FinTrackForWindows.Models.Currency;
using FinTrackWebApi.Dtos.CurrencyDtos;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Currencies
{
    public interface ICurrenciesStore
    {
        ReadOnlyObservableCollection<CurrencyModel> Currencies { get; }
        Task LoadCurrenciesAsync();
        event NotifyCollectionChangedEventHandler? CurrenciesChanged;
        Task<CurrencyHistoryDto?> GetHistoricalDataAsync(string targetCurrencyCode, string period);
        Task<decimal> GetConvertCurrencies(string fromCurrencyCode, string toCurrencyCode, decimal amount);
    }
}
