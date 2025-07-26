using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Transaction;
using FinTrackForWindows.Services.Api;
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

        [ObservableProperty]
        private ObservableCollection<TransactionModel> transactions;

        [ObservableProperty]
        private TransactionModel editableTransaction;

        [ObservableProperty]
        private TransactionModel? selectedTransaction;

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

        public TransactionsViewModel(ILogger<TransactionsViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
            _ = LoadData();

            NewTransaction();
        }

        [RelayCommand]
        private void NewTransaction()
        {
            IsEditing = false;
            EditableTransaction = new TransactionModel();
            _logger.LogInformation("Yeni işlem formu hazırlandı.");

            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        [RelayCommand]
        private void EditTransaction(TransactionModel transactionToEdit)
        {
            if (transactionToEdit == null) return;

            IsEditing = true;

            EditableTransaction = new TransactionModel()
            {
                Id = transactionToEdit.Id,
                NameOrDescription = transactionToEdit.NameOrDescription,
                Amount = transactionToEdit.Amount,
                Type = transactionToEdit.Type,
                Date = transactionToEdit.Date,
                AccountName = transactionToEdit.AccountName,
                Category = transactionToEdit.Category,
                Currency = transactionToEdit.Currency
            };

            _logger.LogInformation("'{TransactionName}' adlı işlem düzenleniyor.", EditableTransaction.NameOrDescription);

            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));
        }


        [RelayCommand]
        private async Task DeleteTransaction(TransactionModel transactionToDelete)
        {
            if (transactionToDelete != null)
            {
                await _apiService.DeleteAsync<object>($"Transactions/{transactionToDelete.Id}");
                Transactions.Remove(transactionToDelete);
                _logger.LogInformation("'{TransactionName}' adlı işlem silindi.", transactionToDelete.NameOrDescription);
                NewTransaction();
            }
        }

        [RelayCommand]
        private async Task SaveTransaction()
        {
            if (string.IsNullOrWhiteSpace(EditableTransaction.NameOrDescription))
            {
                _logger.LogWarning("Kaydetme denemesi başarısız: İşlem açıklaması boş.");
                MessageBox.Show("İşlem açıklaması boş olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsEditing)
            {
                var existing = Transactions.FirstOrDefault(t => t.Id == EditableTransaction.Id);
                if (existing != null)
                {
                    existing.NameOrDescription = EditableTransaction.NameOrDescription;
                    existing.Amount = EditableTransaction.Amount;
                    existing.Type = EditableTransaction.Type;
                    existing.Date = EditableTransaction.Date;
                    existing.AccountName = EditableTransaction.AccountName;
                    existing.Category = EditableTransaction.Category;
                    existing.Currency = EditableTransaction.Currency;

                    // TODO: Yeni Gelir/Gider şemasına uygun.
                    //await _apiService.PutAsync<object>($"Transactions/{existing.Id}", new TransactionUpdateDto 
                    //{

                    //});

                    _logger.LogInformation("'{TransactionName}' adlı işlem güncellendi.", existing.NameOrDescription);
                    CalculateTotals();
                }
            }
            else
            {
                Transactions.Add(EditableTransaction);
                _logger.LogInformation("Yeni işlem eklendi: '{TransactionName}'", EditableTransaction.NameOrDescription);
            }

            NewTransaction();
        }

        private async Task LoadData()
        {
            var transactions = await _apiService.GetAsync<List<TransactionDto>>("Transactions");
            Transactions = new ObservableCollection<TransactionModel>();
            if (transactions != null)
            {
                foreach (var transaction in transactions)
                {
                    Transactions.Add(new TransactionModel
                    {
                        Id = transaction.Id,
                        NameOrDescription = transaction.Description ?? "N/A",
                        Amount = transaction.Amount,
                        Date = transaction.TransactionDateUtc,
                        AccountName = transaction.Account.Name,
                        Category = transaction.Category.Name,
                        Type = transaction.Category.Type,
                        Currency = transaction.Currency.ToString()
                    });
                }
            }

            Transactions.CollectionChanged += OnTransactionsChanged;
            CalculateTotals();
        }

        private void OnTransactionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _logger.LogInformation("Koleksiyon değişti, toplamlar yeniden hesaplanıyor.");
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            TotalIncome = Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount).ToString("C");
            TotalExpense = Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount).ToString("C");
            var mostSpendingTransaction = Transactions
                .OrderByDescending(t => t.Amount)
                .FirstOrDefault();
            MostSpending = mostSpendingTransaction != null
                ? mostSpendingTransaction.Category
                : "Henüz işlem yok";
        }
    }
}