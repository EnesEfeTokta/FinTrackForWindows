using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTrack.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject currentCenterViewModel;

        private readonly TopBarViewModel _topBarViewModel;
        private readonly BottomBarViewModel _bottomBarViewModel;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly BudgetViewModel _budgetViewModel;
        private readonly AccountViewModel _accountViewModel;
        private readonly TransactionsViewModel _transactionsViewModel;
        private readonly DebtViewModel _debtViewModel;
        private readonly CurrenciesViewModel _currenciesViewModel;
        private readonly ReportsViewModel _reportsViewModel;
        private readonly FinBotViewModel _finBotViewModel;
        private readonly FeedbackViewModel _feedbackViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public MainViewModel()
        {
            _topBarViewModel = new TopBarViewModel();
            _bottomBarViewModel = new BottomBarViewModel();
            _dashboardViewModel = new DashboardViewModel();
            _budgetViewModel = new BudgetViewModel();
            _accountViewModel = new AccountViewModel();
            _transactionsViewModel = new TransactionsViewModel();
            _currenciesViewModel = new CurrenciesViewModel();
            _reportsViewModel = new ReportsViewModel();
            _debtViewModel = new DebtViewModel();
            _finBotViewModel = new FinBotViewModel();
            _feedbackViewModel = new FeedbackViewModel();
            _settingsViewModel = new SettingsViewModel();

            // Navigate...
        }
    }
}
