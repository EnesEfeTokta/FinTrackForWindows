using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using FinTrack.Models.Account;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrack.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<AccountModel> accounts;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private AccountModel selectedAccount;

        public string FormTitle => IsEditing ? "Hesabı Düzenle" : "Yeni Hesap Ekle";

        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "HESAP OLUŞTUR";

        private bool IsEditing = false;
        private Guid? editingAccountId = null;

        private readonly ILogger<AccountViewModel> _logger;

        public AccountViewModel(ILogger<AccountViewModel> logger)
        {
            _logger = logger;

            LoadSampleData();
            PrepareForNewAccount();
        }

        private void LoadSampleData()
        {
            Accounts = new ObservableCollection<AccountModel>
            {
                new AccountModel
                {
                    Name = "ING Bank - Vadesiz",
                    Type = AccountType.Checking,
                    CurrentBalance = 15000,
                    TargetBalance = 20000,
                    Currency = "USD"
                },
                new AccountModel
                {
                    Name = "QNB Bank - Kredi Kartı",
                    Type = AccountType.CreditCard,
                    CurrentBalance = 3000,
                    TargetBalance = 10000,
                    Currency = "USD"
                },
                new AccountModel
                {
                    Name = "Yatırım Hesabı - Portföy",
                    Type = AccountType.Loan,
                    CurrentBalance = 90000,
                    TargetBalance = null,
                    Currency = "USD"
                }
            };
        }

        [RelayCommand]
        private void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(SelectedAccount?.Name))
            {
                return;
            }

            if (IsEditing && editingAccountId.HasValue)
            {
                var existingAccount = Accounts.FirstOrDefault(a => a.Id == editingAccountId.Value);
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
            }
        }

        [RelayCommand]
        private void PrepareToEditAccount(AccountModel accountToEdit)
        {
            if (accountToEdit == null) return;

            IsEditing = true;
            editingAccountId = accountToEdit.Id;

            SelectedAccount = new AccountModel
            {
                Id = accountToEdit.Id,
                Name = accountToEdit.Name,
                Type = accountToEdit.Type,
                CurrentBalance = accountToEdit.CurrentBalance,
                TargetBalance = accountToEdit.TargetBalance,
                Currency = accountToEdit.Currency
            };
        }

        private void PrepareForNewAccount()
        {
            IsEditing = false;
            editingAccountId = null;
            SelectedAccount = new AccountModel();
        }
    }
}
