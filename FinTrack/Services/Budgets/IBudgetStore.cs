using FinTrackForWindows.Dtos.BudgetDtos;
using FinTrackForWindows.Models.Budget;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Budgets
{
    public interface IBudgetStore
    {
        ReadOnlyObservableCollection<BudgetModel> Budgets { get; }

        event NotifyCollectionChangedEventHandler? BudgetsChanged;

        Task LoadBudgetsAsync();

        Task AddBudgetAsync(BudgetCreateDto newBudget);
        Task UpdateBudgetAsync(int budgetId, BudgetCreateDto updatedBudget);
        Task UpdateReachedAmountAsync(BudgetUpdateReachedAmountDto newBudget);
        Task DeleteBudgetAsync(int budgetId);
    }
}
