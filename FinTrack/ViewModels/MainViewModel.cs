using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTrack.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject currentCenterViewModel;

        [ObservableProperty]
        private TopBarViewModel topBarViewModel;

        [ObservableProperty]
        private BottomBarViewModel bottomBarViewModel;

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
        private readonly NotificationViewModel _notificationViewModel;

        public MainViewModel()
        {
            topBarViewModel = new TopBarViewModel();
            bottomBarViewModel = new BottomBarViewModel();

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
            _notificationViewModel = new NotificationViewModel();

            topBarViewModel.NavigateToDashboardRequested += () => CurrentCenterViewModel = _dashboardViewModel;
            topBarViewModel.NavigateToAccountRequested += () => CurrentCenterViewModel = _accountViewModel;
            topBarViewModel.NavigateToBudegtRequested += () => CurrentCenterViewModel = _budgetViewModel;
            topBarViewModel.NavigateToTransactionsRequested += () => CurrentCenterViewModel = _transactionsViewModel;
            topBarViewModel.NavigateToCurrenciesRequested += () => CurrentCenterViewModel = _currenciesViewModel;
            topBarViewModel.NavigateToReportsRequested += () => CurrentCenterViewModel = _reportsViewModel;
            topBarViewModel.NavigateToDebtRequested += () => CurrentCenterViewModel = _debtViewModel;
            topBarViewModel.NavigateToFinBotRequested += () => CurrentCenterViewModel = _finBotViewModel;
            topBarViewModel.NavigateToFeedbackRequested += () => CurrentCenterViewModel = _feedbackViewModel;
            topBarViewModel.NavigateToSettingsRequested += () => CurrentCenterViewModel = _settingsViewModel;
            topBarViewModel.NavigateToNotificationRequested += () => CurrentCenterViewModel = _notificationViewModel;

            TopBarViewModel = topBarViewModel;
            CurrentCenterViewModel = _dashboardViewModel;
            BottomBarViewModel = bottomBarViewModel;
        }
    }
}
