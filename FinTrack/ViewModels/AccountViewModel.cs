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
<<<<<<< Updated upstream
        private AccountModel selectedAccount;

        public string FormTitle => IsEditing ? "Hesabı Düzenle" : "Yeni Hesap Ekle";

        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "HESAP OLUŞTUR";

        private bool IsEditing = false;
        private Guid? editingAccountId = null;

        private readonly ILogger<AccountViewModel> _logger;
=======
        private AccountModel? selectedAccount;

        private readonly ILogger<AccountViewModel> _logger;

        public string FormTitle => IsEditing ? "Hesabı Düzenle" : "Yeni Hesap Ekle";
        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "HESAP OLUŞTUR";

        private bool IsEditing = false;
>>>>>>> Stashed changes

        public AccountViewModel(ILogger<AccountViewModel> logger)
        {
            _logger = logger;
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
            LoadSampleData();
            PrepareForNewAccount();
        }

<<<<<<< Updated upstream
        private void LoadSampleData()
        {
=======
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
>>>>>>> Stashed changes
            Accounts = new ObservableCollection<AccountModel>
            {
                new AccountModel
                {
<<<<<<< Updated upstream
=======
                    Id = Guid.NewGuid(),
>>>>>>> Stashed changes
                    Name = "ING Bank - Vadesiz",
                    Type = AccountType.Checking,
                    CurrentBalance = 15000,
                    TargetBalance = 20000,
<<<<<<< Updated upstream
                    Currency = "USD"
                },
                new AccountModel
                {
=======
                    Currency = "USD",
                    History = new List<AccountBalanceHistoryPoint>()
                },
                new AccountModel
                {
                    Id = Guid.NewGuid(),
>>>>>>> Stashed changes
                    Name = "QNB Bank - Kredi Kartı",
                    Type = AccountType.CreditCard,
                    CurrentBalance = 3000,
                    TargetBalance = 10000,
<<<<<<< Updated upstream
                    Currency = "USD"
                },
                new AccountModel
                {
                    Name = "Yatırım Hesabı - Portföy",
                    Type = AccountType.Loan,
                    CurrentBalance = 90000,
                    TargetBalance = null,
                    Currency = "USD"
=======
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
>>>>>>> Stashed changes
                }
            };
        }

        [RelayCommand]
        private void SaveAccount()
        {
<<<<<<< Updated upstream
            if (string.IsNullOrWhiteSpace(SelectedAccount?.Name))
            {
                return;
            }

            if (IsEditing && editingAccountId.HasValue)
            {
                var existingAccount = Accounts.FirstOrDefault(a => a.Id == editingAccountId.Value);
=======
            if (SelectedAccount == null || string.IsNullOrWhiteSpace(SelectedAccount.Name)) return;

            if (IsEditing)
            {
                var existingAccount = Accounts.FirstOrDefault(a => a.Id == SelectedAccount.Id);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                Accounts.Add(SelectedAccount);
            }

=======
                SelectedAccount.Id = Guid.NewGuid();
                Accounts.Add(SelectedAccount);
            }
>>>>>>> Stashed changes
            PrepareForNewAccount();
        }

        [RelayCommand]
        private void DeleteAccount(AccountModel accountToDelete)
        {
            if (accountToDelete != null)
            {
                Accounts.Remove(accountToDelete);
<<<<<<< Updated upstream
=======
                if (SelectedAccount?.Id == accountToDelete.Id)
                {
                    PrepareForNewAccount();
                }
>>>>>>> Stashed changes
            }
        }

        [RelayCommand]
        private void PrepareToEditAccount(AccountModel accountToEdit)
        {
            if (accountToEdit == null) return;
<<<<<<< Updated upstream

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
=======
            SelectedAccount = accountToEdit;
>>>>>>> Stashed changes
        }

        private void PrepareForNewAccount()
        {
<<<<<<< Updated upstream
            IsEditing = false;
            editingAccountId = null;
=======
>>>>>>> Stashed changes
            SelectedAccount = new AccountModel();
        }
    }
}