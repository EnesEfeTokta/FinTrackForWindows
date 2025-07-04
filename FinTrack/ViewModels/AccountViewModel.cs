using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using FinTrack.Models.Account;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace FinTrack.ViewModels
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

        public AccountViewModel(ILogger<AccountViewModel> logger)
        {
            _logger = logger;
            LoadSampleData();
            PrepareForNewAccount();
        }

        partial void OnSelectedAccountChanged(AccountModel? value)
        {
            _logger.LogInformation("Seçilen hesap değişti: {AccountName}", value?.Name ?? "Hiçbiri");

            IsEditing = value != null && value.Id != Guid.Empty;

            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        private void LoadSampleData()
        {
            var today = DateTime.Today;
            Accounts = new ObservableCollection<AccountModel>
            {
                new AccountModel
                {
                    Id = Guid.NewGuid(),
                    Name = "ING Bank - Vadesiz",
                    Type = AccountType.Checking,
                    CurrentBalance = 15000,
                    TargetBalance = 20000,
                    Currency = "USD",
                    History = new List<AccountBalanceHistoryPoint>()
                },
                new AccountModel
                {
                    Id = Guid.NewGuid(),
                    Name = "QNB Bank - Kredi Kartı",
                    Type = AccountType.CreditCard,
                    CurrentBalance = 3000,
                    TargetBalance = 10000,
                    Currency = "USD",
                    History = new List<AccountBalanceHistoryPoint>()
                },
                new AccountModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Yatırım Hesabı - Portföy",
                    Type = AccountType.Investment,
                    CurrentBalance = 90000,
                    TargetBalance = null,
                    Currency = "USD",
                    History = new List<AccountBalanceHistoryPoint>()
                }
            };
        }

        [RelayCommand]
        private void SaveAccount()
        {
            if (SelectedAccount == null || string.IsNullOrWhiteSpace(SelectedAccount.Name)) return;

            if (IsEditing)
            {
                var existingAccount = Accounts.FirstOrDefault(a => a.Id == SelectedAccount.Id);
                if (existingAccount != null)
                {
                    existingAccount.Name = SelectedAccount.Name;
                    existingAccount.Type = SelectedAccount.Type;
                    existingAccount.CurrentBalance = SelectedAccount.CurrentBalance;
                    existingAccount.TargetBalance = SelectedAccount.TargetBalance;
                    existingAccount.Currency = SelectedAccount.Currency;
                }
            }
            else
            {
                SelectedAccount.Id = Guid.NewGuid();
                Accounts.Add(SelectedAccount);
            }
            PrepareForNewAccount();
        }

        [RelayCommand]
        private void DeleteAccount(AccountModel accountToDelete)
        {
            if (accountToDelete != null)
            {
                Accounts.Remove(accountToDelete);
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