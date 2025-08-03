using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Models.Account;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Accounts
{
    public class AccountStore : IAccountStore
    {
        private readonly IApiService _apiService;

        private readonly ILogger<AccountStore> _logger;

        private readonly ObservableCollection<AccountModel> _accounts;

        public ReadOnlyObservableCollection<AccountModel> Accounts { get; }

        public event NotifyCollectionChangedEventHandler? AccountsChanged;

        public AccountStore(IApiService apiService, ILogger<AccountStore> logger)
        {
            _apiService = apiService;
            _logger = logger;
            _accounts = new ObservableCollection<AccountModel>();
            Accounts = new ReadOnlyObservableCollection<AccountModel>(_accounts);

            _accounts.CollectionChanged += OnInternalCollectionChanged;
        }

        public async Task LoadAccountsAsync()
        {
            if (_accounts.Any())
            {
                _logger.LogInformation("Hesaplar zaten yüklü. API çağrısı atlanıyor.");
                return;
            }

            try
            {
                var AccountsFromApi = await _apiService.GetAsync<List<AccountDto>>("Account");
                if (AccountsFromApi == null) return;

                _accounts.Clear();
                foreach (var dto in AccountsFromApi)
                {
                    _accounts.Add(new AccountModel
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Type = dto.Type,
                        Currency = dto.Currency,
                        Balance = dto.Balance,
                    });
                }
                _logger.LogInformation("{Count} adet hesap Accountstore'a yüklendi.", _accounts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Accountstore'da hesaplar yüklenirken hata oluştu.");
            }
        }

        public async Task AddAccountAsync(AccountCreateDto newAccountDto)
        {
            var createdAccountDto = await _apiService.PostAsync<AccountDto>("Account", newAccountDto);
            if (createdAccountDto != null)
            {
                _accounts.Add(new AccountModel
                {
                    Id = createdAccountDto.Id,
                    Name = createdAccountDto.Name,
                    Currency = createdAccountDto.Currency,
                    Type = createdAccountDto.Type,
                });
            }
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            await _apiService.DeleteAsync<bool>($"Account/{accountId}");
            var accountToRemove = _accounts.FirstOrDefault(b => b.Id == accountId);
            if (accountToRemove != null)
            {
                _accounts.Remove(accountToRemove);
            }
        }

        public async Task UpdateAccountAsync(int accountId, AccountCreateDto updatedAccount)
        {
            var updateAccountDto = await _apiService.PutAsync<AccountDto>($"Account/{accountId}", updatedAccount);
            if (updatedAccount != null)
            {
                foreach (var item in _accounts)
                {
                    if (item.Id == accountId)
                    {
                        item.Name = updatedAccount.Name;
                        item.Currency = updatedAccount.Currency;
                        item.Type = updatedAccount.Type;
                    }
                }
            }
        }

        private void OnInternalCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AccountsChanged?.Invoke(this, e);
        }
    }
}
