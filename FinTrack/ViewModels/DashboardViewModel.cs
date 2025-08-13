using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Core;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Account;
using FinTrackForWindows.Models.Budget;
using FinTrackForWindows.Models.Dashboard;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Models.Transaction;
using FinTrackForWindows.Services.Accounts;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Budgets;
using FinTrackForWindows.Services.Currencies;
using FinTrackForWindows.Services.Debts;
using FinTrackForWindows.Services.Memberships;
using FinTrackForWindows.Services.Reports;
using FinTrackForWindows.Services.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace FinTrackForWindows.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetDashboardModel>? _budgets_DashboardView_ItemsControl;

        [ObservableProperty]
        private ObservableCollection<CurrencyRateDashboard>? _currencyRates_DashboardView_ItemsControl;

        [ObservableProperty]
        private ObservableCollection<AccountDashboard>? _accounts_DashboardView_ItemsControl;

        [ObservableProperty]
        private string _totalBalance_DashboardView_TextBlock = string.Empty;

        [ObservableProperty]
        private string _totalBalanceIntegerPart = "0";

        [ObservableProperty]
        private string _totalBalanceFractionalPart = ",00";

        [ObservableProperty]
        private string _totalBalanceCurrency = "TRY";

        [ObservableProperty]
        private ObservableCollection<TransactionDashboard>? _transactions_DashboardView_ListView;

        [ObservableProperty]
        private MembershipDashboard? _currentMembership_DashboardView_Multiple;

        [ObservableProperty]
        private DebtDashboard? _currentDebt_DashboardView_Multiple;

        [ObservableProperty]
        private ObservableCollection<ReportDashboardModel>? _reports_DashboardView_ItemsControl;

        [ObservableProperty]
        private string transactionSummary = string.Empty;

        [ObservableProperty]
        private DebtModel _currentActiveDebt;

        private readonly ILogger<DashboardViewModel> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly IBudgetStore _budgetStore;
        private readonly IAccountStore _accountStore;
        private readonly ITransactionStore _transactionStore;
        private readonly ICurrenciesStore _currenciesStore;
        private readonly IMembershipStore _membershipStore;
        private readonly IDebtStore _debtStore;
        private readonly IReportStore _reportStore;
        private readonly IAppInNotificationService _appInNotificationService;

        public IEnumerable<BudgetModel> DashboardBudgets => _budgetStore.Budgets.Take(4);
        public IEnumerable<AccountModel> DashboardAccounts => _accountStore.Accounts.Take(2);
        public IEnumerable<TransactionModel> DashboardTransactions => _transactionStore.Transactions.Take(50);
        public IEnumerable<DebtModel> DashboardDebts => _debtStore.MyDebtsList.Take(5);

        public event NotifyCollectionChangedEventHandler? BudgetsChanged;
        public event NotifyCollectionChangedEventHandler? AccountsChanges;
        public event NotifyCollectionChangedEventHandler? TransactionsChanges;
        public event NotifyCollectionChangedEventHandler? MembershipChanges;
        public event NotifyCollectionChangedEventHandler? DebtsChanges;

        private DispatcherTimer _debtCarouselTimer;
        private int _currentDebtIndex = 0;

        public DashboardViewModel(
            ILogger<DashboardViewModel> logger,
            IServiceProvider serviceProvider,
            IBudgetStore budgetStore,
            IAccountStore accountStore,
            ITransactionStore transactionStore,
            ICurrenciesStore currenciesStore,
            IMembershipStore membershipStore,
            IDebtStore debtStore,
            IReportStore reportStore,
            IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _budgetStore = budgetStore;
            _accountStore = accountStore;
            _transactionStore = transactionStore;
            _currenciesStore = currenciesStore;
            _membershipStore = membershipStore;
            _debtStore = debtStore;
            _reportStore = reportStore;
            _appInNotificationService = appInNotificationService;

            _budgetStore.BudgetsChanged += OnBudgetsChanged;
            _accountStore.AccountsChanged += OnAccountsChanged;
            _transactionStore.TransactionsChanged += OnTransactionsChanged;
            _membershipStore.CurrentUserMembershipChanged += OnMembershipChanged;
            _debtStore.DebtsChanged += OnDebtsChanged;
            _currenciesStore.CurrenciesChanged += OnCurrenciesChanged;

            if (SessionManager.IsLoggedIn)
            {
                _logger.LogInformation("The user is already logged in. DashboardViewModel is loading data.");
                _ = LoadInitialDataAsync();
            }
        }

        private async Task LoadInitialDataAsync()
        {
            RefreshDashboardBudgets();
            RefreshDashboardAccounts();
            RefreshDashboardTransactions();
            RefreshDashboardCurrencies();
            RefreshDashboardMembership();

            LoadReportData();

            await CalculateTotalBalance();
        }

        private void RefreshDashboardBudgets()
        {
            Budgets_DashboardView_ItemsControl = new ObservableCollection<BudgetDashboardModel>();

            foreach (var budget in DashboardBudgets)
            {
                Budgets_DashboardView_ItemsControl.Add(new BudgetDashboardModel
                {
                    Name = budget.Name,
                    DueDate = budget.EndDate.ToString("dd.MM.yyyy"),
                    Amount = $"{budget.AllocatedAmount} {budget.Currency}",
                    RemainingTime = $"Only {(budget.EndDate - budget.StartDate).Days} days left.",
                    StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush")
                });
            }
        }

        private void OnBudgetsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("BudgetStore değişti, Dashboard UI yenileniyor.");
                RefreshDashboardBudgets();
            });
        }

        // ------

        private void RefreshDashboardAccounts()
        {
            var newAccountList = new ObservableCollection<AccountDashboard>();
            _logger.LogInformation("Refreshing dashboard accounts...");

            if (_transactionStore.Transactions == null || !_transactionStore.Transactions.Any())
            {
                _logger.LogWarning("Transaction store is empty. Account income/expense ratios cannot be calculated.");
            }

            foreach (var account in DashboardAccounts)
            {
                var accountTransactions = _transactionStore.Transactions
                    .Where(t => t.AccountId == account.Id)
                    .ToList();

                _logger.LogInformation("Account '{AccountName}' (ID: {AccountId}): Found {TransactionCount} transactions.",
                                        account.Name, account.Id, accountTransactions.Count);

                decimal totalIncome = accountTransactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                decimal totalExpense = accountTransactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                decimal grandTotal = totalIncome + totalExpense;

                double incomePercentage = 0;
                double expensePercentage = 0;

                if (grandTotal > 0)
                {
                    incomePercentage = (double)(totalIncome / grandTotal * 100);
                    expensePercentage = (double)(totalExpense / grandTotal * 100);
                }
                else if (account.Balance > 0 && !accountTransactions.Any())
                {
                    incomePercentage = 100;
                }

                _logger.LogInformation("Account '{AccountName}': Income={Income}, Expense={Expense}, Income%={IncomePerc}, Expense%={ExpensePerc}",
                                        account.Name, totalIncome, totalExpense, incomePercentage, expensePercentage);

                newAccountList.Add(new AccountDashboard
                {
                    Name = account.Name,
                    Balance = $"{account.Balance:N2} {account.Currency}",
                    IncomePercentage = incomePercentage,
                    ExpensePercentage = expensePercentage,
                    IncomeAmountText = $"+{totalIncome:N2}",
                    ExpenseAmountText = $"-{totalExpense:N2}"
                });
            }

            Accounts_DashboardView_ItemsControl = newAccountList;
            _logger.LogInformation("Dashboard accounts UI model updated.");
        }

        private void OnAccountsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("AccountStore değişti, Dashboard UI yenileniyor.");
                RefreshDashboardAccounts();
            });

            _ = CalculateTotalBalance();
        }

        private async Task CalculateTotalBalance()
        {
            if (_accountStore.Accounts == null || !_accountStore.Accounts.Any())
            {
                TotalBalanceIntegerPart = "0";
                TotalBalanceFractionalPart = ",00";
                TotalBalanceCurrency = "TRY";
                return;
            }

            string targetCurrency = "TRY";
            decimal totalBalanceInTargetCurrency = 0;

            var conversionTasks = new List<Task<decimal>>();
            foreach (var account in _accountStore.Accounts)
            {
                decimal balance = account.Balance ?? 0;
                string accountCurrency = account.Currency.ToString();
                if (balance == 0) continue;
                if (accountCurrency == targetCurrency)
                {
                    totalBalanceInTargetCurrency += balance;
                }
                else
                {
                    conversionTasks.Add(
                        _currenciesStore.GetConvertCurrencies(accountCurrency, targetCurrency, balance)
                    );
                }
            }

            var convertedAmounts = await Task.WhenAll(conversionTasks);
            totalBalanceInTargetCurrency += convertedAmounts.Sum();

            string formattedBalance = totalBalanceInTargetCurrency.ToString("N2");

            var parts = formattedBalance.Split(',');
            if (parts.Length == 2)
            {
                TotalBalanceIntegerPart = parts[0];
                TotalBalanceFractionalPart = "," + parts[1];
            }
            else
            {
                TotalBalanceIntegerPart = formattedBalance;
                TotalBalanceFractionalPart = ",00";
            }

            TotalBalanceCurrency = targetCurrency;
        }

        // ------

        private void RefreshDashboardTransactions()
        {
            Transactions_DashboardView_ListView = new ObservableCollection<TransactionDashboard>();

            foreach (var transaction in DashboardTransactions)
            {
                Transactions_DashboardView_ListView.Add(new TransactionDashboard
                {
                    DateText = transaction.Date.ToString("dd.MM.yyyy"),
                    Description = transaction.NameOrDescription ?? "N/A",
                    Amount = $"{transaction.Amount} {transaction.Currency}",
                    Category = transaction.CategoryName,
                    Type = transaction.Type,
                });
            }

            double totalIncome = Transactions_DashboardView_ListView
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t =>
                {
                    var cleaned = t.Amount.Replace("+", string.Empty).Replace("$", string.Empty).Trim();
                    return double.TryParse(cleaned, out var value) ? value : 0;
                });
            double totalExpense = Transactions_DashboardView_ListView
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t =>
                {
                    var cleaned = t.Amount.Replace("-", string.Empty).Replace("$", string.Empty).Trim();
                    return double.TryParse(cleaned, out var value) ? value : 0;
                });
            double remainingBalance = totalIncome - totalExpense;

            TransactionSummary =
                $"A total of {Transactions_DashboardView_ListView.Count} transactions found. " +
                $"Income: +{totalIncome} {Transactions_DashboardView_ListView.FirstOrDefault()?.Amount?.Split(' ')?.LastOrDefault() ?? ""}, " +
                $"Expense: -{totalExpense} {Transactions_DashboardView_ListView.FirstOrDefault()?.Amount?.Split(' ')?.LastOrDefault() ?? ""} " +
                $"Remaining: {remainingBalance} {Transactions_DashboardView_ListView.FirstOrDefault()?.Amount?.Split(' ')?.LastOrDefault() ?? ""}";
        }

        private void OnTransactionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("TransactionsStore değişti, Dashboard UI yenileniyor.");
                RefreshDashboardTransactions();
            });
        }

        // ------

        private void RefreshDashboardCurrencies()
        {
            CurrencyRates_DashboardView_ItemsControl = new ObservableCollection<CurrencyRateDashboard>();

            var tryRate = _currenciesStore.Currencies.FirstOrDefault(c => c.ToCurrencyCode == "TRY");
            var eurRate = _currenciesStore.Currencies.FirstOrDefault(c => c.ToCurrencyCode == "EUR");

            if (tryRate != null)
            {
                CurrencyRates_DashboardView_ItemsControl.Add(new CurrencyRateDashboard
                {
                    FromCurrencyName = "USD",
                    ToCurrencyName = tryRate.ToCurrencyCode,
                    ToCurrencyAmount = tryRate.ToCurrencyPrice.ToString("N4"),
                    FromCurrencyFlagUrl = "https://flagcdn.com/w320/us.png",
                    ToCurrencyFlagUrl = tryRate.ToCurrencyFlag,
                    FromCurrencyAmount = "1.00",
                    ToCurrencyCountry = tryRate.ToCurrencyName,
                    FromCurrencyCountry = "United States",
                    ToCurrencyImageHeight = 20
                });
            }

            if (eurRate != null)
            {
                CurrencyRates_DashboardView_ItemsControl.Add(new CurrencyRateDashboard
                {
                    FromCurrencyName = "USD",
                    ToCurrencyName = eurRate.ToCurrencyCode,
                    ToCurrencyAmount = eurRate.ToCurrencyPrice.ToString("N4"),
                    FromCurrencyFlagUrl = "https://flagcdn.com/w320/us.png",
                    ToCurrencyFlagUrl = eurRate.ToCurrencyFlag,
                    FromCurrencyAmount = "1.00",
                    ToCurrencyCountry = eurRate.ToCurrencyName,
                    FromCurrencyCountry = "Euro Area",
                    ToCurrencyImageHeight = 20
                });
            }
        }

        private void OnCurrenciesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("CurrenciesStore changed, refreshing dashboard currencies and total balance.");
                RefreshDashboardCurrencies();
                _ = CalculateTotalBalance();
            });
        }

        // ------

        private void RefreshDashboardMembership()
        {
            var currentMembership = _membershipStore.CurrentUserMembership;

            if (currentMembership == null)
            {
                CurrentMembership_DashboardView_Multiple = null;
                return;
            }

            var correspondingPlan = _membershipStore.AvailablePlans.FirstOrDefault(p => p.Id == currentMembership.PlanId);

            string price = correspondingPlan?.Price.ToString() ?? "N/A" + " " + correspondingPlan?.Currency;

            CurrentMembership_DashboardView_Multiple = new MembershipDashboard
            {
                Level = $"{currentMembership.PlanName} | {currentMembership.Status.ToUpper()}",
                StartDate = currentMembership.StartDate.ToString("dd.MM.yyyy"),
                RenewalDate = currentMembership.EndDate.ToString("dd.MM.yyyy"),
                Price = price,
            };
        }

        private void OnMembershipChanged()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("Membershipstore değişti, Dashboard UI yenileniyor.");
                RefreshDashboardMembership();
            });
        }

        // ------

        private void OnDebtsChanged()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _logger.LogInformation("DebtStore has changed, Dashboard debts are being renewed.");
                SetupDebtCarousel();
            });
        }

        private void SetupDebtCarousel()
        {
            _debtCarouselTimer?.Stop();

            if (_debtStore.ActiveDebts != null && _debtStore.ActiveDebts.Any())
            {
                _currentDebtIndex = 0;
                CurrentActiveDebt = _debtStore.ActiveDebts[_currentDebtIndex];

                if (_debtStore.ActiveDebts.Count > 1)
                {
                    _debtCarouselTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(5)
                    };
                    _debtCarouselTimer.Tick += DebtCarouselTimer_Tick;
                    _debtCarouselTimer.Start();
                }
            }
            else
            {
                CurrentActiveDebt = null;
            }
        }

        private void DebtCarouselTimer_Tick(object sender, EventArgs e)
        {
            if (_debtStore.ActiveDebts == null || _debtStore.ActiveDebts.Count == 0)
            {
                _debtCarouselTimer.Stop();
                CurrentActiveDebt = null;
                return;
            }

            _currentDebtIndex++;
            if (_currentDebtIndex >= _debtStore.ActiveDebts.Count)
            {
                _currentDebtIndex = 0;
            }

            CurrentActiveDebt = _debtStore.ActiveDebts[_currentDebtIndex];
        }

        // ------

        private void LoadReportData()
        {
            Reports_DashboardView_ItemsControl = new ObservableCollection<ReportDashboardModel>
            {
                CreateReport("2025 Yılı Hesap Raporu", ReportType.Account),
                CreateReport("2025 Yılı Gelir-Gider Raporu", ReportType.Transaction),
                CreateReport("2025 Yılı Bütçe Raporu", ReportType.Budget),
            };
        }

        private ReportDashboardModel CreateReport(string name, ReportType type)
        {
            var reportLogger = _serviceProvider.GetRequiredService<ILogger<ReportDashboardModel>>();
            var report = new ReportDashboardModel(reportLogger, _reportStore)
            {
                Name = name,
                Type = type
            };
            return report;
        }
    }
}
