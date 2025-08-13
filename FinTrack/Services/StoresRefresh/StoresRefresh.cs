using FinTrackForWindows.Services.Accounts;
using FinTrackForWindows.Services.Budgets;
using FinTrackForWindows.Services.Currencies;
using FinTrackForWindows.Services.Debts;
using FinTrackForWindows.Services.Memberships;
using FinTrackForWindows.Services.Transactions;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.Services.StoresRefresh
{
    public class StoresRefresh : IStoresRefresh
    {
        private readonly IAccountStore _accountStore;
        private readonly IBudgetStore _budgetStore;
        private readonly ITransactionStore _transactionStore;
        private readonly IDebtStore _debtStore;
        private readonly IMembershipStore _membershipStore;
        private readonly ICurrenciesStore _currenciesStore;

        private readonly ILogger<StoresRefresh> _logger;

        public event Action? RefreshStarted;
        public event Action<bool>? RefreshCompleted;

        public StoresRefresh(IAccountStore accountStore,
            IBudgetStore budgetStore,
            ITransactionStore transactionStore,
            IDebtStore debtStore,
            IMembershipStore membershipStore,
            ICurrenciesStore currenciesStore,
            ILogger<StoresRefresh> logger)
        {
            _accountStore = accountStore;
            _budgetStore = budgetStore;
            _transactionStore = transactionStore;
            _debtStore = debtStore;
            _membershipStore = membershipStore;
            _currenciesStore = currenciesStore;
            _logger = logger;
        }

        public async Task<bool> RefreshAllStoresAsync()
        {
            _logger.LogInformation("Refresh process starting for all data stores...");
            RefreshStarted?.Invoke();

            try
            {
                Task loadAccountsTask = _accountStore.LoadAccountsAsync();
                Task loadBudgetsTask = _budgetStore.LoadBudgetsAsync();
                Task loadDebtsTask = _debtStore.LoadDebtsAsync();
                Task loadMembershipTask = _membershipStore.LoadAllMembershipDataAsync();
                Task loadTransactionsTask = _transactionStore.LoadTransactionsAsync();
                Task loadCurrenciesTask = _currenciesStore.LoadCurrenciesAsync();

                await Task.WhenAll(
                    loadAccountsTask,
                    loadBudgetsTask,
                    loadDebtsTask,
                    loadMembershipTask,
                    loadCurrenciesTask,
                    loadTransactionsTask);

                _logger.LogInformation("All data stores have been successfully refreshed.");
                RefreshCompleted?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing the data stores.");
                RefreshCompleted?.Invoke(false);
                return false;
            }
        }
    }
}
