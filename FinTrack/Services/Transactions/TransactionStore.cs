using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Models.Transaction;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Transactions
{
    public class TransactionStore : ITransactionStore
    {
        private readonly IApiService _apiService;

        private readonly ILogger<TransactionStore> _logger;

        private readonly ObservableCollection<TransactionModel> _transactions;

        public ReadOnlyObservableCollection<TransactionModel> Transactions { get; }

        public TransactionStore(IApiService apiService, ILogger<TransactionStore> logger)
        {
            _apiService = apiService;
            _logger = logger;
            _transactions = new ObservableCollection<TransactionModel>();
            Transactions = new ReadOnlyObservableCollection<TransactionModel>(_transactions);

            _transactions.CollectionChanged += OnInternalCollectionChanged;
        }

        public event NotifyCollectionChangedEventHandler? TransactionsChanged;

        public async Task AddTransactionAsync(TransactionCreateDto newTransaction)
        {
            var createdTransactionDto = await _apiService.PostAsync<TransactionDto>("Transactions", newTransaction);
            if (createdTransactionDto != null)
            {
                _transactions.Add(new TransactionModel
                {
                    Id = createdTransactionDto.Id,
                    AccountId = createdTransactionDto.Account.Id,
                    CategoryId = createdTransactionDto.Category.Id,
                    NameOrDescription = createdTransactionDto.Description ?? "N/A",
                    Amount = createdTransactionDto.Amount,
                    Type = createdTransactionDto.Category.Type,
                    Date = createdTransactionDto.TransactionDateUtc.ToLocalTime(),
                    AccountName = createdTransactionDto.Account.Name,
                    CategoryName = createdTransactionDto.Category.Name,
                    Currency = createdTransactionDto.Currency,
                });
            }
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            await _apiService.DeleteAsync<object>($"Transactions/{transactionId}");
            var transactionToRemove = _transactions.FirstOrDefault(b => b.Id == transactionId);
            if (transactionToRemove != null)
            {
                _transactions.Remove(transactionToRemove);
            }
        }

        public async Task LoadTransactionsAsync()
        {
            if (_transactions.Any())
            {
                _logger.LogInformation("İşlemler zaten yüklü. API çağrısı atlanıyor.");
                return;
            }

            try
            {
                var TransactionFromApi = await _apiService.GetAsync<List<TransactionDto>>("Transactions");
                if (TransactionFromApi == null) return;

                _transactions.Clear();
                foreach (var dto in TransactionFromApi)
                {
                    _transactions.Add(new TransactionModel
                    {
                        Id = dto.Id,
                        NameOrDescription = dto.Description ?? "N/A",
                        Amount = dto.Amount,
                        Date = dto.TransactionDateUtc,
                        AccountName = dto.Account.Name,
                        CategoryId = dto.Category.Id,
                        CategoryName = dto.Category.Name,
                        Type = dto.Category.Type,
                        Currency = dto.Currency
                    });
                }
                _logger.LogInformation("{Count} adet işlem Transactionstore'a yüklendi.", _transactions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transactionstore'da işlemler yüklenirken hata oluştu.");
            }
        }

        public async Task UpdateTransactionAsync(int transactionId, TransactionCreateDto updatedTransaction)
        {
            var updateTransactionDto = await _apiService.PutAsync<TransactionDto>($"Transactions/{transactionId}", updatedTransaction);
            if (updateTransactionDto != null)
            {
                var itemToUpdate = _transactions.FirstOrDefault(t => t.Id == transactionId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.NameOrDescription = updateTransactionDto.Description ?? "N/A";
                    itemToUpdate.Amount = updateTransactionDto.Amount;
                    itemToUpdate.Type = updateTransactionDto.Category.Type;
                    itemToUpdate.Date = updateTransactionDto.TransactionDateUtc.ToLocalTime();
                    itemToUpdate.AccountName = updateTransactionDto.Account.Name;
                    itemToUpdate.CategoryName = updateTransactionDto.Category.Name;
                    itemToUpdate.Currency = updateTransactionDto.Currency;
                    itemToUpdate.AccountId = updateTransactionDto.Account.Id;
                    itemToUpdate.CategoryId = updateTransactionDto.Category.Id;
                }
            }
        }

        private void OnInternalCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            TransactionsChanged?.Invoke(this, e);
        }
    }
}
