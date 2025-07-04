using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Models.Budget;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrack.ViewModels
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

        private readonly ILogger<BudgetViewModel> _logger;

        public string FormTitle => IsEditing ? "Bütçeyi Düzenle" : "Yeni Bütçe Ekle";
        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "BÜTÇE OLUŞTUR";

        public BudgetViewModel(ILogger<BudgetViewModel> logger)
        {
            _logger = logger;
            LoadSampleData();
            PrepareForNewBudget();
        }

        private void LoadSampleData()
        {
            Budgets = new ObservableCollection<BudgetModel>
            {
                new BudgetModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Yemek Bütçesi",
                    Amount = 850,
                    TargetAmount = 1200,
                    StartDate = DateTime.Today.AddDays(-15),
                    EndDate = DateTime.Today.AddDays(15),
                    Currency = "TRY"
                },
                new BudgetModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Ulaşım Bütçesi",
                    Amount = 300,
                    TargetAmount = 600,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddMonths(1),
                    Currency = "TRY"
                }
            };
        }

        [RelayCommand]
        private void SaveBudget()
        {
            if (SelectedBudget == null || string.IsNullOrWhiteSpace(SelectedBudget.Name))
            {
                _logger.LogWarning("Bütçe kaydedilemedi: Gerekli alanlar doldurulmamış.");
                MessageBox.Show("Lütfen bütçe adını ve hedef tutarını doldurun.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsEditing)
            {
                var existingBudget = Budgets.FirstOrDefault(b => b.Id == SelectedBudget.Id);
                if (existingBudget != null)
                {
                    existingBudget.Name = SelectedBudget.Name;
                    existingBudget.TargetAmount = SelectedBudget.TargetAmount;
                    existingBudget.StartDate = SelectedBudget.StartDate;
                    existingBudget.EndDate = SelectedBudget.EndDate;
                    existingBudget.Currency = SelectedBudget.Currency;

                    _logger.LogInformation("Bütçe güncellendi: {BudgetName}", existingBudget.Name);
                }
            }
            else
            {
                SelectedBudget.Id = Guid.NewGuid();
                Budgets.Add(SelectedBudget);
                _logger.LogInformation("Yeni bütçe eklendi: {BudgetName}", SelectedBudget.Name);
            }
            PrepareForNewBudget();
        }

        [RelayCommand]
        private void DeleteBudget(BudgetModel? budgetToDelete)
        {
            if (budgetToDelete == null)
            {
                _logger.LogWarning("Silinecek bütçe bulunamadı.");
                return;
            }

            Budgets.Remove(budgetToDelete);
            _logger.LogInformation("Bütçe silindi: {BudgetName}", budgetToDelete.Name);

            if (SelectedBudget?.Id == budgetToDelete.Id)
            {
                PrepareForNewBudget();
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
                Amount = budgetToEdit.Amount,
                TargetAmount = budgetToEdit.TargetAmount,
                StartDate = budgetToEdit.StartDate,
                EndDate = budgetToEdit.EndDate,
                Currency = budgetToEdit.Currency
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
            SelectedBudget = new BudgetModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };
            IsEditing = false;
        }
    }
}