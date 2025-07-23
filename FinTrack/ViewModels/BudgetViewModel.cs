// FinTrackForWindows.ViewModels/BudgetViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.BudgetDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Budget;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class BudgetViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetModel> budgets;

        [ObservableProperty]
        private BudgetModel? selectedBudget;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private bool isEditing;

        public ObservableCollection<string> Categories { get; }
        public IEnumerable<BaseCurrencyType> CurrencyTypes => Enum.GetValues(typeof(BaseCurrencyType)).Cast<BaseCurrencyType>();

        public string FormTitle => IsEditing ? "Edit Budget" : "Add New Budget";
        public string SaveButtonText => IsEditing ? "UPDATE THE BUDGET" : "CREATE A BUDGET";

        private readonly ILogger<BudgetViewModel> _logger;
        private readonly IApiService _apiService;

        public BudgetViewModel(ILogger<BudgetViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            Budgets = new ObservableCollection<BudgetModel>();
            Categories = new ObservableCollection<string> { "Gıda", "Fatura", "Ulaşım", "Eğlence", "Sağlık", "Diğer" };

            _ = LoadBudgetsAsync();
            PrepareForNewBudget();
        }

        private async Task LoadBudgetsAsync()
        {
            try
            {
                var budgetsFromApi = await _apiService.GetAsync<List<BudgetDto>>("Budgets");
                if (budgetsFromApi == null) return;

                Budgets.Clear();
                foreach (var dto in budgetsFromApi)
                {
                    Budgets.Add(new BudgetModel
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Description = dto.Description,
                        Category = dto.Category,
                        AllocatedAmount = dto.AllocatedAmount,
                        CurrentAmount = 0, // TODO: API'den gelen veride mevcut tutar yoksa 0 olarak ayarla
                        Currency = dto.Currency,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bütçeler yüklenirken bir hata oluştu.");
                MessageBox.Show("Bütçeler yüklenemedi. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SaveBudgetAsync()
        {
            if (SelectedBudget == null || string.IsNullOrWhiteSpace(SelectedBudget.Name) || SelectedBudget.AllocatedAmount <= 0)
            {
                MessageBox.Show("Lütfen bütçe adı ve 0'dan büyük bir hedef tutar girin.", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var budgetDto = new BudgetCreateDto
            {
                Name = SelectedBudget.Name,
                Description = SelectedBudget.Description,
                Category = SelectedBudget.Category,
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
                    // TEST
                    await _apiService.PutAsync<BudgetModel>($"Budgets/{selectedBudget.Id}", budgetDto);
                    _logger.LogInformation("Bütçe güncellendi: {BudgetId}", selectedBudget.Id);

                    var existingBudget = Budgets.FirstOrDefault(b => b.Id == SelectedBudget.Id);
                    if (existingBudget != null)
                    {
                        var index = Budgets.IndexOf(existingBudget);
                        Budgets[index] = SelectedBudget;
                    }
                }
                else
                {
                    var createdBudgetDto = await _apiService.PostAsync<BudgetDto>("Budgets", budgetDto);
                    _logger.LogInformation("Yeni bütçe oluşturuldu.");
                    Budgets.Add(SelectedBudget);
                }
                PrepareForNewBudget();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bütçe kaydedilirken bir hata oluştu.");
                MessageBox.Show("Bütçe kaydedilemedi.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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
                await _apiService.DeleteAsync<BudgetModel>($"Budgets/{budgetToDelete.Id}");
                Budgets.Remove(budgetToDelete);
                _logger.LogInformation("Bütçe silindi: {BudgetId}", budgetToDelete.Id);

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