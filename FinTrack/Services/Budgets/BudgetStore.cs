using FinTrackForWindows.Dtos.BudgetDtos;
using FinTrackForWindows.Models.Budget;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Budgets
{
    public class BudgetStore : IBudgetStore
    {
        private readonly IApiService _apiService;

        private readonly ILogger<BudgetStore> _logger;

        private readonly ObservableCollection<BudgetModel> _budgets;

        public ReadOnlyObservableCollection<BudgetModel> Budgets { get; }

        public event NotifyCollectionChangedEventHandler? BudgetsChanged;

        public BudgetStore(IApiService apiService, ILogger<BudgetStore> logger)
        {
            _apiService = apiService;
            _logger = logger;
            _budgets = new ObservableCollection<BudgetModel>();
            Budgets = new ReadOnlyObservableCollection<BudgetModel>(_budgets);

            _budgets.CollectionChanged += OnInternalCollectionChanged;
        }

        public async Task LoadBudgetsAsync()
        {
            if (_budgets.Any())
            {
                _logger.LogInformation("Budgets are already loaded. Skipping API call.");
                return;
            }

            try
            {
                var budgetsFromApi = await _apiService.GetAsync<List<BudgetDto>>("Budgets");
                if (budgetsFromApi == null) return;

                _budgets.Clear();
                foreach (var dto in budgetsFromApi)
                {
                    _budgets.Add(new BudgetModel
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Description = dto.Description,
                        Category = dto.Category,
                        AllocatedAmount = dto.AllocatedAmount,
                        ReachedAmount = dto.ReachedAmount,
                        Currency = dto.Currency,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate,
                    });
                }
                _logger.LogInformation("{Count} budgets loaded into BudgetStore.", _budgets.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading budgets in BudgetStore.");
            }
        }

        public async Task AddBudgetAsync(BudgetCreateDto newBudgetDto)
        {
            var createdBudgetDto = await _apiService.PostAsync<BudgetDto>("Budgets", newBudgetDto);
            if (createdBudgetDto != null)
            {
                _budgets.Add(new BudgetModel
                {
                    Id = createdBudgetDto.Id,
                    Name = createdBudgetDto.Name,
                    Description = createdBudgetDto.Description,
                    Category = createdBudgetDto.Category,
                    AllocatedAmount = createdBudgetDto.AllocatedAmount,
                    ReachedAmount = createdBudgetDto.ReachedAmount,
                    Currency = createdBudgetDto.Currency,
                    StartDate = createdBudgetDto.StartDate,
                    EndDate = createdBudgetDto.EndDate
                });
            }
        }

        public async Task DeleteBudgetAsync(int budgetId)
        {
            await _apiService.DeleteAsync<bool>($"Budgets/{budgetId}");
            var budgetToRemove = _budgets.FirstOrDefault(b => b.Id == budgetId);
            if (budgetToRemove != null)
            {
                _budgets.Remove(budgetToRemove);
            }
        }

        public async Task UpdateBudgetAsync(int budgetId, BudgetCreateDto updatedBudget)
        {
            var updateBudgetDto = await _apiService.PutAsync<BudgetDto>($"Budgets/{budgetId}", updatedBudget);
            if (updateBudgetDto != null)
            {
                foreach (var item in _budgets)
                {
                    if (item.Id == budgetId)
                    {
                        item.Name = updatedBudget.Name;
                        item.Description = updatedBudget.Description;
                        item.Category = updatedBudget.Category;
                        item.AllocatedAmount = updatedBudget.AllocatedAmount;
                        item.ReachedAmount = updatedBudget.ReachedAmount;
                        item.Currency = updatedBudget.Currency;
                        item.StartDate = updatedBudget.StartDate;
                        item.EndDate = updatedBudget.EndDate;
                    }
                }
            }
        }

        public async Task UpdateReachedAmountAsync(BudgetUpdateReachedAmountDto updatedBudget)
        {
            var updateBudgetDto = await _apiService.PutAsync<BudgetDto>($"Budgets/Update-Reached-Amount", updatedBudget);
            if (updateBudgetDto != null)
            {
                foreach (var item in _budgets)
                {
                    if (item.Id == updateBudgetDto.Id)
                    {
                        item.ReachedAmount = updatedBudget.ReachedAmount;
                    }
                }
            }
        }

        private void OnInternalCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            BudgetsChanged?.Invoke(this, e);
        }
    }
}
