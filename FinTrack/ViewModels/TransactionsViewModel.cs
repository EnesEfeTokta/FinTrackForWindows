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
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using SkiaSharp;
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

        private static readonly SKColor PrimaryColor = new SKColor(100, 181, 246);
        private static readonly SKColor SecondaryColor = new SKColor(239, 83, 80);
        private static readonly SKColor[] PieChartColors = new[]
        {
            new SKColor(100, 181, 246), // Mavi
            new SKColor(239, 83, 80),   // Kırmızı  
            new SKColor(255, 167, 38),  // Turuncu
            new SKColor(129, 199, 132), // Yeşil
            new SKColor(171, 71, 188),  // Mor
            new SKColor(255, 193, 7),   // Sarı
            new SKColor(96, 125, 139),  // Gri-Mavi
            new SKColor(233, 30, 99),   // Pembe
            new SKColor(156, 39, 176),  // Menekşe
            new SKColor(63, 81, 181)    // İndigo
        };

        public ReadOnlyObservableCollection<TransactionModel> Transactions => _transactionStore.Transactions;
        public ObservableCollection<TransactionModel> FilteredTransactions { get; }
        public ReadOnlyObservableCollection<AccountModel> AllAccounts => _accountStore.Accounts;

        [ObservableProperty]
        private ObservableCollection<TransactionCategoryModel> allCategories;
        public ObservableCollection<TransactionCategoryModel> CategoriesForFilter { get; }

        [ObservableProperty]
        private string? filterByDescription;

        [ObservableProperty]
        private string? filterByTransactionType;

        [ObservableProperty]
        private DateTime? filterByStartDate;

        [ObservableProperty]
        private DateTime? filterByEndDate;

        [ObservableProperty]
        private string? filterByMinAmount;

        [ObservableProperty]
        private string? filterByMaxAmount;

        [ObservableProperty]
        private TransactionCategoryModel? filterByCategory;
        public ObservableCollection<string> TransactionTypeFilterOptions { get; }
        private const string AllTypesFilter = "Tüm Türler";

        [ObservableProperty]
        private TransactionModel editableTransaction;

        [ObservableProperty]
        private TransactionModel? selectedTransaction;

        [ObservableProperty]
        private AccountModel? selectedAccountForForm;

        private TransactionCategoryModel? _selectedCategoryForForm;
        public TransactionCategoryModel? SelectedCategoryForForm
        {
            get => _selectedCategoryForForm;
            set
            {
                SetProperty(ref _selectedCategoryForForm, value);
                if (value != null && EditableTransaction != null) { EditableTransaction.Type = value.Type; }
            }
        }

        public ObservableCollection<BaseCurrencyType> CurrencyTypes { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveTransactionCommand))]
        private bool isEditing;

        public string FormTitle => IsEditing ? "İşlemi Düzenle" : "Yeni İşlem Ekle";
        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "İŞLEM OLUŞTUR";

        [ObservableProperty]
        private ISeries[] incomeVsExpenseSeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private ISeries[] incomeByCategorySeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private ISeries[] expenseByCategorySeries = Array.Empty<ISeries>();

        public TransactionsViewModel(ILogger<TransactionsViewModel> logger, IApiService apiService,
                                     ITransactionStore transactionStore, IAccountStore accountStore)
        {
            _logger = logger;
            _apiService = apiService;
            _transactionStore = transactionStore;
            _accountStore = accountStore;


            FilteredTransactions = new ObservableCollection<TransactionModel>();
            AllCategories = new ObservableCollection<TransactionCategoryModel>();
            CategoriesForFilter = new ObservableCollection<TransactionCategoryModel>();

            TransactionTypeFilterOptions = new ObservableCollection<string>
            {
                AllTypesFilter,
                "Gelir",
                "Gider"
            };

            foreach (BaseCurrencyType currencyType in Enum.GetValues(typeof(BaseCurrencyType)))
            {
                CurrencyTypes.Add(currencyType);
            }

            _transactionStore.TransactionsChanged += OnTransactionsChanged;

            _ = LoadData();
            NewTransaction();
        }

        partial void OnFilterByDescriptionChanged(string? value) => ApplyFilters();
        partial void OnFilterByTransactionTypeChanged(string? value) => ApplyFilters();
        partial void OnFilterByStartDateChanged(DateTime? value) => ApplyFilters();
        partial void OnFilterByEndDateChanged(DateTime? value) => ApplyFilters();
        partial void OnFilterByMinAmountChanged(string? value) => ApplyFilters();
        partial void OnFilterByMaxAmountChanged(string? value) => ApplyFilters();
        partial void OnFilterByCategoryChanged(TransactionCategoryModel? value) => ApplyFilters();

        private void ApplyFilters()
        {
            _logger.LogInformation("ApplyFilters başladı...");

            if (_transactionStore?.Transactions == null)
            {
                _logger.LogWarning("TransactionStore.Transactions null!");
                return;
            }

            var allTransactions = _transactionStore.Transactions.ToList();
            _logger.LogInformation($"Store'dan alınan toplam işlem sayısı: {allTransactions.Count}");

            var filtered = allTransactions.AsParallel()
                .Where(t =>
                {
                    bool passes = true;
                    if (!string.IsNullOrWhiteSpace(FilterByDescription))
                        passes &= t.NameOrDescription?.Contains(FilterByDescription, StringComparison.OrdinalIgnoreCase) == true;
                    if (FilterByStartDate.HasValue)
                        passes &= t.Date.Date >= FilterByStartDate.Value.Date;
                    if (FilterByEndDate.HasValue)
                        passes &= t.Date.Date <= FilterByEndDate.Value.Date;
                    if (!string.IsNullOrEmpty(FilterByTransactionType) && FilterByTransactionType != AllTypesFilter)
                        passes &= t.Type == (FilterByTransactionType == "Gelir" ? TransactionType.Income : TransactionType.Expense);
                    if (FilterByCategory != null && FilterByCategory.Id != -1)
                        passes &= t.CategoryId == FilterByCategory.Id;
                    if (decimal.TryParse(FilterByMinAmount, out var minAmount))
                        passes &= t.Amount >= minAmount;
                    if (decimal.TryParse(FilterByMaxAmount, out var maxAmount) && maxAmount > 0)
                        passes &= t.Amount <= maxAmount;
                    return passes;
                })
                .OrderByDescending(t => t.Date)
                .ToList();

            _logger.LogInformation($"Filtrelenmiş işlem sayısı: {filtered.Count}");

            FilteredTransactions.Clear();
            foreach (var transaction in filtered)
            {
                FilteredTransactions.Add(transaction);
            }

            _logger.LogInformation($"FilteredTransactions güncellendi, eleman sayısı: {FilteredTransactions.Count}");
            CalculateTotals();
        }

        [RelayCommand]
        private void ResetFilters()
        {
            FilterByDescription = null;
            FilterByTransactionType = AllTypesFilter;
            FilterByStartDate = null;
            FilterByEndDate = null;
            FilterByMinAmount = null;
            FilterByMaxAmount = null;
            FilterByCategory = CategoriesForFilter.FirstOrDefault(c => c.Id == -1);
            ApplyFilters();
        }

        private async Task LoadCategories()
        {
            var categoryDtos = await _apiService.GetAsync<List<TransactionCategoryDto>>("TransactionCategory");

            AllCategories.Clear();
            CategoriesForFilter.Clear();

            var allCatOption = new TransactionCategoryModel { Id = -1, Name = "Tüm Kategoriler" };
            CategoriesForFilter.Add(allCatOption);

            if (categoryDtos != null)
            {
                foreach (var dto in categoryDtos.OrderBy(c => c.Name))
                {
                    var model = new TransactionCategoryModel { Id = dto.Id, Name = dto.Name, Type = dto.Type };
                    AllCategories.Add(model);
                    CategoriesForFilter.Add(model);
                }
            }

            FilterByCategory = allCatOption;
            _logger.LogInformation("{Count} adet kategori yüklendi.", AllCategories.Count);
        }

        private void OnTransactionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _logger.LogInformation("İşlem listesi değişti, filtreler ve toplamlar yeniden uygulanıyor.");
            if (App.Current.Dispatcher.CheckAccess())
            {
                ApplyFilters();
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ApplyFilters();
                }));
            }
        }

        private void CalculateTotals()
        {
            _logger.LogInformation($"CalculateTotals başladı. FilteredTransactions Count: {FilteredTransactions?.Count ?? 0}");

            if (FilteredTransactions == null || !FilteredTransactions.Any())
            {
                _logger.LogWarning("FilteredTransactions boş veya null!");
                IncomeVsExpenseSeries = Array.Empty<ISeries>();
                IncomeByCategorySeries = Array.Empty<ISeries>();
                ExpenseByCategorySeries = Array.Empty<ISeries>();
                return;
            }

            var allTransactions = FilteredTransactions.ToList();
            var incomeTransactions = allTransactions.Where(t => t.Type == TransactionType.Income).ToList();
            var expenseTransactions = allTransactions.Where(t => t.Type == TransactionType.Expense).ToList();

            var incomeTotal = incomeTransactions.Sum(t => t.Amount);
            var expenseTotal = expenseTransactions.Sum(t => t.Amount);

            _logger.LogInformation($"Gelir toplamı: {incomeTotal}, Gider toplamı: {expenseTotal}");

            var incomeVsExpenseList = new List<ISeries>();

            if (incomeTotal > 0)
            {
                incomeVsExpenseList.Add(new PieSeries<ObservableValue>
                {
                    Name = "Gelir",
                    Values = new[] { new ObservableValue((double)incomeTotal) },
                    Fill = new SolidColorPaint(PrimaryColor),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = p => $"₺{p.Coordinate.PrimaryValue:N0}",
                    DataLabelsSize = 14
                });
            }

            if (expenseTotal > 0)
            {
                incomeVsExpenseList.Add(new PieSeries<ObservableValue>
                {
                    Name = "Gider",
                    Values = new[] { new ObservableValue((double)expenseTotal) },
                    Fill = new SolidColorPaint(SecondaryColor),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = p => $"₺{p.Coordinate.PrimaryValue:N0}",
                    DataLabelsSize = 14
                });
            }

            IncomeVsExpenseSeries = incomeVsExpenseList.ToArray();
            _logger.LogInformation($"Ana grafik oluşturuldu, eleman sayısı: {incomeVsExpenseList.Count}");

            var incomeByCategory = incomeTransactions
                .Where(t => t.Amount > 0)
                .GroupBy(t => string.IsNullOrEmpty(t.CategoryName) ? "Kategorisiz" : t.CategoryName)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total)
                .ToList();

            _logger.LogInformation($"Gelir kategorileri: {incomeByCategory.Count} adet");

            var incomeSeries = new List<ISeries>();
            if (incomeByCategory.Any())
            {
                for (int i = 0; i < incomeByCategory.Count; i++)
                {
                    var item = incomeByCategory[i];
                    var color = PieChartColors[i % PieChartColors.Length];

                    incomeSeries.Add(new PieSeries<ObservableValue>
                    {
                        Name = item.Category,
                        Values = new[] { new ObservableValue((double)item.Total) },
                        Fill = new SolidColorPaint(color),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = p => $"₺{p.Coordinate.PrimaryValue:N0}",
                        DataLabelsSize = 11,
                        MiniatureShapeSize = 10
                    });

                    _logger.LogInformation($"Gelir Kategorisi eklendi: {item.Category} - ₺{item.Total:N0}");
                }
            }
            else
            {
                incomeSeries.Add(new PieSeries<ObservableValue>
                {
                    Name = "Veri Yok",
                    Values = new[] { new ObservableValue(1) },
                    Fill = new SolidColorPaint(SKColors.LightGray),
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkGray),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = p => "Gelir Yok"
                });
            }

            IncomeByCategorySeries = incomeSeries.ToArray();
            _logger.LogInformation($"Gelir kategorileri grafiği oluşturuldu: {incomeSeries.Count} seri");

            var expenseByCategory = expenseTransactions
                .Where(t => t.Amount > 0)
                .GroupBy(t => string.IsNullOrEmpty(t.CategoryName) ? "Kategorisiz" : t.CategoryName)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total)
                .ToList();

            _logger.LogInformation($"Gider kategorileri: {expenseByCategory.Count} adet");

            var expenseSeries = new List<ISeries>();
            if (expenseByCategory.Any())
            {
                for (int i = 0; i < expenseByCategory.Count; i++)
                {
                    var item = expenseByCategory[i];
                    var color = PieChartColors[i % PieChartColors.Length];

                    expenseSeries.Add(new PieSeries<ObservableValue>
                    {
                        Name = item.Category,
                        Values = new[] { new ObservableValue((double)item.Total) },
                        Fill = new SolidColorPaint(color),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = p => $"₺{p.Coordinate.PrimaryValue:N0}",
                        DataLabelsSize = 11,
                        MiniatureShapeSize = 10
                    });

                    _logger.LogInformation($"Gider Kategorisi eklendi: {item.Category} - ₺{item.Total:N0}");
                }
            }
            else
            {
                expenseSeries.Add(new PieSeries<ObservableValue>
                {
                    Name = "Veri Yok",
                    Values = new[] { new ObservableValue(1) },
                    Fill = new SolidColorPaint(SKColors.LightGray),
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkGray),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = p => "Gider Yok"
                });
            }

            ExpenseByCategorySeries = expenseSeries.ToArray();
            _logger.LogInformation($"Gider kategorileri grafiği oluşturuldu: {expenseSeries.Count} seri");

            OnPropertyChanged(nameof(IncomeVsExpenseSeries));
            OnPropertyChanged(nameof(IncomeByCategorySeries));
            OnPropertyChanged(nameof(ExpenseByCategorySeries));

            _logger.LogInformation($"Tüm grafikler güncellendi - Gelir: ₺{incomeTotal:N2}, Gider: ₺{expenseTotal:N2}");
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
    }
}