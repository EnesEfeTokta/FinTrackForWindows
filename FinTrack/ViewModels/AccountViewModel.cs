using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Account;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<AccountModel> accounts;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private AccountModel? selectedAccount;

        private readonly ILogger<AccountViewModel> _logger;

        public string FormTitle => IsEditing ? "Hesabı Düzenle" : "Yeni Hesap Ekle";
        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "HESAP OLUŞTUR";

        private bool IsEditing = false;

        private readonly IApiService _apiService;

        public AccountViewModel(ILogger<AccountViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
            _ = LoadData();
            PrepareForNewAccount();
        }

        partial void OnSelectedAccountChanged(AccountModel? value)
        {
            _logger.LogInformation("Seçilen hesap değişti: {AccountName}", value?.Name ?? "Hiçbiri");

            IsEditing = value != null && value.Id != null;

            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        private async Task LoadData()
        {
            var accounts = await _apiService.GetAsync<List<AccountDto>>("Account");
            Accounts = new ObservableCollection<AccountModel>();
            if (accounts != null)
            {
                foreach (var item in accounts)
                {
                    Accounts.Add(new AccountModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Type = item.Type,
                        Balance = item.Balance,
                        Currency = item.Currency.ToString(),
                    });
                }
            }
        }

        [RelayCommand]
        private async Task SaveAccount()
        {
            if (SelectedAccount == null || string.IsNullOrWhiteSpace(SelectedAccount.Name)) return;

            if (Enum.TryParse(SelectedAccount.Currency, out BaseCurrencyType currency))
            {
                _logger.LogInformation("Seçilen para birimi: {Currency}", currency);
            }
            else
            {
                _logger.LogWarning("Geçersiz para birimi: {Currency}", SelectedAccount.Currency);
                return;
            }

            if (IsEditing)
            {
                var existingAccount = Accounts.FirstOrDefault(a => a.Id == SelectedAccount.Id);
                if (existingAccount != null)
                {
                    existingAccount.Name = SelectedAccount.Name;
                    existingAccount.Type = SelectedAccount.Type;
                    existingAccount.Balance = SelectedAccount.Balance;
                    existingAccount.Currency = SelectedAccount.Currency;

                    await _apiService.PutAsync<object>($"Account/{SelectedAccount.Id}", new AccountUpdateDto
                    {
                        Name = SelectedAccount.Name,
                        Type = SelectedAccount.Type,
                        Balance = SelectedAccount.Balance,
                        Currency = currency
                    });
                }
            }
            else
            {
                var newAccount = await _apiService.PostAsync<AccountResponseDto>("Account", new AccountCreateDto
                {
                    Name = SelectedAccount.Name,
                    Type = SelectedAccount.Type,
                    IsActive = true,
                    Balance = SelectedAccount.Balance,
                    Currency = currency,
                });

                SelectedAccount.Id = newAccount.Id;

                Accounts.Add(SelectedAccount);
            }
            PrepareForNewAccount();
        }

        [RelayCommand]
        private async Task DeleteAccount(AccountModel accountToDelete)
        {
            if (accountToDelete != null)
            {
                Accounts.Remove(accountToDelete);

               await _apiService.DeleteAsync<object>($"Account/{accountToDelete.Id}");

                if (SelectedAccount?.Id == accountToDelete.Id)
                {
                    PrepareForNewAccount();
                }
            }
        }

        [RelayCommand]
        private void PrepareToEditAccount(AccountModel accountToEdit)
        {
            if (accountToEdit == null) return;
            SelectedAccount = accountToEdit;
        }

        private void PrepareForNewAccount()
        {
            SelectedAccount = new AccountModel();
        }
    }
}