using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.BudgetDtos;
using FinTrackForWindows.Dtos.CategoryDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Budget;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.Services.Budgets;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class BudgetViewModel : ObservableObject
    {
        public ReadOnlyObservableCollection<BudgetModel> Budgets => _budgetStore.Budgets;

        [ObservableProperty]
        private BudgetModel? selectedBudget;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private bool isEditing;

        public ObservableCollection<string> Categories { get; }
        public IEnumerable<BaseCurrencyType> CurrencyTypes => Enum.GetValues(typeof(BaseCurrencyType)).Cast<BaseCurrencyType>();
        public string FormTitle => IsEditing ? "Bütçeyi Düzenle" : "Yeni Bütçe Ekle";
        public string SaveButtonText => IsEditing ? "BÜTÇEYİ GÜNCELLE" : "BÜTÇE OLUŞTUR";

        private readonly IBudgetStore _budgetStore;
        private readonly ILogger<BudgetViewModel> _logger;
        private readonly IApiService _apiService;

        public BudgetViewModel(IBudgetStore budgetStore, ILogger<BudgetViewModel> logger, IApiService apiService)
        {
            _budgetStore = budgetStore;
            _logger = logger;
            _apiService = apiService;

            Categories = new ObservableCollection<string>();

            _ = InitializeViewModelAsync();
            PrepareForNewBudget();
        }

        private async Task InitializeViewModelAsync()
        {
            await _budgetStore.LoadBudgetsAsync();
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categoriesFromApi = await _apiService.GetAsync<List<CategoryDto>>("Categories");
                if (categoriesFromApi == null) return;

                App.Current.Dispatcher.Invoke(() =>
                {
                    Categories.Clear();
                    foreach (var category in categoriesFromApi.OrderBy(c => c.Name))
                    {
                        Categories.Add(category.Name);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategoriler yüklenirken hata oluştu.");
                MessageBox.Show("Kategoriler yüklenemedi. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SaveBudgetAsync()
        {
            if (SelectedBudget == null || string.IsNullOrWhiteSpace(SelectedBudget.Name) || SelectedBudget.AllocatedAmount <= 0)
            {
                MessageBox.Show("Lütfen bütçe adı ve sıfırdan büyük bir miktar girin.", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? categoryToSave = SelectedBudget.Category?.Trim();
            if (string.IsNullOrWhiteSpace(categoryToSave))
            {
                MessageBox.Show("Lütfen bir kategori seçin veya yazın.", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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

                await LoadCategoriesAsync();
                PrepareForNewBudget();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bütçe kaydedilirken hata oluştu.");
                MessageBox.Show("Bütçe kaydedilemedi. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _logger.LogError(ex, "Bütçe silinirken hata oluştu: {BudgetId}", budgetToDelete.Id);
                MessageBox.Show("Bütçe silinemedi.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void CleanForm()
        {
            PrepareForNewBudget();
        }

        private void PrepareForNewBudget()
        {
            SelectedBudget = new BudgetModel();
            IsEditing = false;
        }
    }
}