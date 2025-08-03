using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Account;
using FinTrackForWindows.Models.Transaction;
using FinTrackForWindows.Services.Accounts;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.Services.Transactions;
using FinTrackForWindows.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly ILogger<TransactionsViewModel> _logger;
        private readonly IApiService _apiService;
        private readonly ITransactionStore _transactionStore;
        private readonly IAccountStore _accountStore;

        public ReadOnlyObservableCollection<TransactionModel> Transactions => _transactionStore.Transactions;
        public ReadOnlyObservableCollection<AccountModel> AllAccounts => _accountStore.Accounts;

        [ObservableProperty]
        private ObservableCollection<TransactionCategoryModel> allCategories;

        [ObservableProperty]
        private TransactionModel editableTransaction;

        [ObservableProperty]
        private TransactionModel? selectedTransaction;

        [ObservableProperty]
        private AccountModel? selectedAccountForForm;

        public ObservableCollection<BaseCurrencyType> CurrencyTypes { get; } = new();

        private TransactionCategoryModel? _selectedCategoryForForm;
        public TransactionCategoryModel? SelectedCategoryForForm
        {
            get => _selectedCategoryForForm;
            set
            {
                SetProperty(ref _selectedCategoryForForm, value);
                if (value != null && EditableTransaction != null)
                {
                    EditableTransaction.Type = value.Type;
                }
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveTransactionCommand))]
        private bool isEditing;

        public string FormTitle => IsEditing ? "İşlemi Düzenle" : "Yeni İşlem Ekle";
        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "İŞLEM OLUŞTUR";

        [ObservableProperty]
        private string totalIncome;

        [ObservableProperty]
        private string totalExpense;

        [ObservableProperty]
        private string mostSpending;

        public TransactionsViewModel(ILogger<TransactionsViewModel> logger, IApiService apiService,
                                     ITransactionStore transactionStore, IAccountStore accountStore)
        {
            _logger = logger;
            _apiService = apiService;
            _transactionStore = transactionStore;
            _accountStore = accountStore;

            AllCategories = new ObservableCollection<TransactionCategoryModel>();

            foreach (BaseCurrencyType currencyType in Enum.GetValues(typeof(BaseCurrencyType)))
            {
                CurrencyTypes.Add(currencyType);
            }

            _transactionStore.TransactionsChanged += OnTransactionsChanged;

            _ = LoadData();
            NewTransaction();
        }

        [RelayCommand]
        private void NewTransaction()
        {
            IsEditing = false;
            EditableTransaction = new TransactionModel { Date = DateTime.Now };
            SelectedCategoryForForm = null;
            SelectedAccountForForm = null;

            _logger.LogInformation("Yeni işlem formu hazırlandı.");

            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        [RelayCommand]
        private void PrepareToEditTransaction(TransactionModel transactionToEdit)
        {
            if (transactionToEdit == null) return;

            IsEditing = true;

            EditableTransaction = new TransactionModel()
            {
                Id = transactionToEdit.Id,
                AccountId = transactionToEdit.AccountId,
                NameOrDescription = transactionToEdit.NameOrDescription,
                Amount = transactionToEdit.Amount,
                Type = transactionToEdit.Type,
                Date = transactionToEdit.Date,
                AccountName = transactionToEdit.AccountName,
                CategoryId = transactionToEdit.CategoryId,
                CategoryName = transactionToEdit.CategoryName,
                Currency = transactionToEdit.Currency
            };

            SelectedCategoryForForm = AllCategories.FirstOrDefault(c => c.Id == transactionToEdit.CategoryId);
            SelectedAccountForForm = AllAccounts.FirstOrDefault(a => a.Id == transactionToEdit.AccountId);

            _logger.LogInformation("'{TransactionName}' adlı işlem düzenlenmek üzere forma yüklendi.", EditableTransaction.NameOrDescription);

            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        [RelayCommand]
        private async Task DeleteTransaction(TransactionModel? transactionToDelete)
        {
            if (transactionToDelete == null) return;

            var result = MessageBox.Show($"'{transactionToDelete.NameOrDescription}' işlemini silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            await _transactionStore.DeleteTransactionAsync(transactionToDelete.Id);
            NewTransaction();
        }

        [RelayCommand]
        private async Task SaveTransaction()
        {
            if (string.IsNullOrWhiteSpace(EditableTransaction.NameOrDescription) ||
                SelectedAccountForForm == null ||
                SelectedCategoryForForm == null)
            {
                MessageBox.Show("Lütfen tüm zorunlu alanları (açıklama, hesap, kategori) doldurun.", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var transactionDto = new TransactionCreateDto
            {
                Description = EditableTransaction.NameOrDescription,
                Amount = EditableTransaction.Amount,
                TransactionDateUtc = EditableTransaction.Date.ToUniversalTime(),
                AccountId = SelectedAccountForForm?.Id ?? -1,
                CategoryId = SelectedCategoryForForm.Id,
                Currency = EditableTransaction.Currency
            };

            if (IsEditing)
            {
                await _transactionStore.UpdateTransactionAsync(EditableTransaction.Id, transactionDto);
                _logger.LogInformation("'{TransactionName}' adlı işlem güncellendi.", EditableTransaction.NameOrDescription);
            }
            else
            {
                await _transactionStore.AddTransactionAsync(transactionDto);
                _logger.LogInformation("Yeni işlem eklendi: '{TransactionName}'", EditableTransaction.NameOrDescription);
            }

            NewTransaction();
        }

        [RelayCommand]
        private async Task AddNewCategory()
        {
            var addCategoryWindow = new AddCategoryWindow(_apiService);

            if (addCategoryWindow.ShowDialog() == true)
            {
                _logger.LogInformation("Yeni kategori başarıyla eklendi, liste yenileniyor.");
                await LoadCategories();

                var newCategory = addCategoryWindow.GetCreatedCategory();
                if (newCategory != null)
                {
                    SelectedCategoryForForm = AllCategories.FirstOrDefault(c => c.Id == newCategory.Id);
                }
            }
        }

        private async Task LoadData()
        {
            await _accountStore.LoadAccountsAsync();
            await LoadCategories();
            await _transactionStore.LoadTransactionsAsync();
        }

        private async Task LoadCategories()
        {
            var categoryDtos = await _apiService.GetAsync<List<TransactionCategoryDto>>("TransactionCategory");

            AllCategories.Clear();
            if (categoryDtos != null)
            {
                foreach (var dto in categoryDtos)
                {
                    AllCategories.Add(new TransactionCategoryModel { Id = dto.Id, Name = dto.Name, Type = dto.Type });
                }
            }
            _logger.LogInformation("{Count} adet kategori yüklendi.", AllCategories.Count);
        }

        private void OnTransactionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _logger.LogInformation("İşlem listesi değişti, toplamlar yeniden hesaplanıyor.");
            App.Current.Dispatcher.Invoke(CalculateTotals);
        }

        private void CalculateTotals()
        {
            if (Transactions == null) return;

            TotalIncome = Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount).ToString("C");

            TotalExpense = Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount).ToString("C");

            var mostSpendingTransaction = Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .OrderByDescending(t => t.Amount)
                .FirstOrDefault();

            MostSpending = mostSpendingTransaction?.CategoryName ?? "Henüz işlem yok";
        }
    }
}