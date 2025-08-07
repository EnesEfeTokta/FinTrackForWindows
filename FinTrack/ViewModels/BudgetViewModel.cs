using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.BudgetDtos;
using FinTrackForWindows.Dtos.CategoryDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Budget;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Budgets;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class BudgetViewModel : ObservableObject
    {
        private const string AllCategoriesFilter = "All Categories";

        public ReadOnlyObservableCollection<BudgetModel> Budgets => _budgetStore.Budgets;
        public ObservableCollection<BudgetModel> FilteredBudgets { get; }

        [ObservableProperty]
        private BudgetModel? selectedBudget;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private bool isEditing;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFilteredBudgets))]
        private bool isFilterActive; 

        [ObservableProperty] private string? filterByName;
        [ObservableProperty] private string? filterByMinAmount;
        [ObservableProperty] private string? filterByMaxAmount;
        [ObservableProperty] private string? selectedFilterCategory;

        public bool HasFilteredBudgets => FilteredBudgets.Any();

        public ObservableCollection<string> CategoriesForForm { get; }
        public ObservableCollection<string> CategoriesForFilter { get; }

        public IEnumerable<BaseCurrencyType> CurrencyTypes => Enum.GetValues(typeof(BaseCurrencyType)).Cast<BaseCurrencyType>();
        public string FormTitle => IsEditing ? "Bütçeyi Düzenle" : "Yeni Bütçe Ekle";
        public string SaveButtonText => IsEditing ? "BÜTÇEYİ GÜNCELLE" : "BÜTÇE OLUŞTUR";

        private readonly IBudgetStore _budgetStore;
        private readonly ILogger<BudgetViewModel> _logger;
        private readonly IApiService _apiService;
        private readonly IAppInNotificationService _notificationService;

        public BudgetViewModel(IBudgetStore budgetStore, ILogger<BudgetViewModel> logger, IApiService apiService, IAppInNotificationService appInNotificationService)
        {
            _budgetStore = budgetStore;
            _logger = logger;
            _apiService = apiService;
            _notificationService = appInNotificationService;

            FilteredBudgets = new ObservableCollection<BudgetModel>();
            CategoriesForForm = new ObservableCollection<string>();
            CategoriesForFilter = new ObservableCollection<string>();

            FilteredBudgets.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasFilteredBudgets));

            _budgetStore.BudgetsChanged += (s, e) => ApplyFilters();

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            await LoadCategoriesAsync();
            await _budgetStore.LoadBudgetsAsync();

            PrepareForNewBudget();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categoriesFromApi = await _apiService.GetAsync<List<CategoryDto>>("Categories");
                if (categoriesFromApi == null) return;

                App.Current.Dispatcher.Invoke(() =>
                {
                    CategoriesForForm.Clear();
                    CategoriesForFilter.Clear();
                    CategoriesForFilter.Add(AllCategoriesFilter);

                    foreach (var category in categoriesFromApi.OrderBy(c => c.Name))
                    {
                        CategoriesForForm.Add(category.Name);
                        CategoriesForFilter.Add(category.Name);
                    }
                    SelectedFilterCategory = AllCategoriesFilter;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategoriler yüklenirken bir hata oluştu.");
                _notificationService.ShowError("Kategoriler yüklenemedi. Lütfen internet bağlantınızı kontrol edin.");
            }
        }

        partial void OnFilterByNameChanged(string? value) => ApplyFilters();
        partial void OnFilterByMinAmountChanged(string? value) => ApplyFilters();
        partial void OnFilterByMaxAmountChanged(string? value) => ApplyFilters();
        partial void OnSelectedFilterCategoryChanged(string? value) => ApplyFilters();

        private void ApplyFilters()
        {
            IEnumerable<BudgetModel> filtered = _budgetStore.Budgets;
            bool activeFilter = false;

            if (!string.IsNullOrWhiteSpace(FilterByName))
            {
                filtered = filtered.Where(b => b.Name.Contains(FilterByName, StringComparison.OrdinalIgnoreCase));
                activeFilter = true;
            }

            if (!string.IsNullOrEmpty(SelectedFilterCategory) && SelectedFilterCategory != AllCategoriesFilter)
            {
                filtered = filtered.Where(b => b.Category.Equals(SelectedFilterCategory, StringComparison.OrdinalIgnoreCase));
                activeFilter = true;
            }

            if (decimal.TryParse(FilterByMinAmount, out var minAmount))
            {
                filtered = filtered.Where(b => b.AllocatedAmount >= minAmount);
                activeFilter = true;
            }

            if (decimal.TryParse(FilterByMaxAmount, out var maxAmount) && maxAmount > 0)
            {
                filtered = filtered.Where(b => b.AllocatedAmount <= maxAmount);
                activeFilter = true;
            }

            IsFilterActive = activeFilter;

            FilteredBudgets.Clear();
            foreach (var budget in filtered.OrderByDescending(b => b.StartDate))
            {
                FilteredBudgets.Add(budget);
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            FilterByName = string.Empty;
            FilterByMinAmount = string.Empty;
            FilterByMaxAmount = string.Empty;
            SelectedFilterCategory = AllCategoriesFilter;
        }

        [RelayCommand]
        private async Task SaveBudgetAsync()
        {
            if (SelectedBudget == null || string.IsNullOrWhiteSpace(SelectedBudget.Name) || SelectedBudget.AllocatedAmount <= 0)
            {
                _notificationService.ShowWarning("Bütçe adı ve tutarı boş veya sıfırdan küçük olamaz.");
                return;
            }

            string? categoryToSave = SelectedBudget.Category?.Trim();
            if (string.IsNullOrWhiteSpace(categoryToSave))
            {
                _notificationService.ShowWarning("Lütfen bir kategori seçin veya yazın.");
                return;
            }

            if (!CategoriesForForm.Contains(categoryToSave))
            {
                CategoriesForForm.Add(categoryToSave);
                CategoriesForFilter.Add(categoryToSave);
            }

            var budgetDto = new BudgetCreateDto
            {
                Name = SelectedBudget.Name,
                Description = SelectedBudget.Description,
                Category = categoryToSave,
                AllocatedAmount = SelectedBudget.AllocatedAmount,
                Currency = SelectedBudget.Currency,
                StartDate = SelectedBudget.StartDate,
                EndDate = SelectedBudget.EndDate,
                IsActive = true
            };

            try
            {
                if (IsEditing)
                {
                    await _budgetStore.UpdateBudgetAsync(SelectedBudget.Id, budgetDto);
                }
                else
                {
                    await _budgetStore.AddBudgetAsync(budgetDto);
                }

                PrepareForNewBudget();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bütçe kaydedilirken bir hata oluştu.");
                _notificationService.ShowError("Bütçe kaydedilemedi. Lütfen internet bağlantınızı kontrol edin.");
            }
        }

        [RelayCommand]
        private async Task DeleteBudgetAsync(BudgetModel? budgetToDelete)
        {
            if (budgetToDelete == null) return;

            var result = MessageBox.Show($"'{budgetToDelete.Name}' adlı bütçeyi silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            try
            {
                await _budgetStore.DeleteBudgetAsync(budgetToDelete.Id);

                if (SelectedBudget?.Id == budgetToDelete.Id)
                {
                    PrepareForNewBudget();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bütçe silinirken bir hata oluştu: {BudgetId}", budgetToDelete.Id);
                _notificationService.ShowError("Bütçe silinemedi. Lütfen internet bağlantınızı kontrol edin.");
            }
        }

        [RelayCommand]
        private void PrepareToEditBudget(BudgetModel? budgetToEdit)
        {
            if (budgetToEdit == null) return;

            SelectedBudget = new BudgetModel
            {
                Id = budgetToEdit.Id,
                Name = budgetToEdit.Name,
                Description = budgetToEdit.Description,
                Category = budgetToEdit.Category,
                AllocatedAmount = budgetToEdit.AllocatedAmount,
                CurrentAmount = budgetToEdit.CurrentAmount,
                Currency = budgetToEdit.Currency,
                StartDate = budgetToEdit.StartDate,
                EndDate = budgetToEdit.EndDate
            };
            IsEditing = true;
        }

        [RelayCommand]
        private void PrepareForNewBudget()
        {
            SelectedBudget = new BudgetModel();
            IsEditing = false;
        }
    }
}