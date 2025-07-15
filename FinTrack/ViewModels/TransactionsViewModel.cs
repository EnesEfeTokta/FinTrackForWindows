using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Transaction;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly ILogger<TransactionsViewModel> _logger;

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

        public TransactionsViewModel(ILogger<TransactionsViewModel> logger)
        {
            _logger = logger;
            LoadSampleData();

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
        private void DeleteTransaction(TransactionModel transactionToDelete)
        {
            if (transactionToDelete != null)
            {
                Transactions.Remove(transactionToDelete);
                _logger.LogInformation("'{TransactionName}' adlı işlem silindi.", transactionToDelete.NameOrDescription);
                NewTransaction();
            }
        }

        [RelayCommand]
        private void SaveTransaction()
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

        private void LoadSampleData()
        {
            Transactions = new ObservableCollection<TransactionModel>
            {
                new TransactionModel
                {
                    NameOrDescription = "Maaş",
                    Amount = 5000m,
                    Date = DateTime.Now.AddDays(-2),
                    AccountName = "ING Bank - Vadesiz",
                    Category = "Maaş",
                    Type = TransactionType.Income,
                    Currency = "TRY"
                },
                new TransactionModel
                {
                    NameOrDescription = "Market Alışverişi",
                    Amount = 450.75m,
                    Date = DateTime.Now.AddDays(-5),
                    AccountName = "Garanti Kredi Kartı",
                    Category = "Gıda",
                    Type = TransactionType.Expense,
                    Currency = "TRY"
                },
                new TransactionModel
                {
                    NameOrDescription = "Elektrik Faturası",
                    Amount = 275.50m,
                    Date = DateTime.Now.AddDays(-10),
                    AccountName = "ING Bank - Vadesiz",
                    Category = "Fatura",
                    Type = TransactionType.Expense,
                    Currency = "TRY"
                }
            };

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