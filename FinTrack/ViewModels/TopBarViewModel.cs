using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FinTrack.ViewModels
{
    public partial class TopBarViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _userAvatar_TopBarView_Image;

        [ObservableProperty]
        private string? _userFullName_TopBarView_TextBlock;

        [ObservableProperty]
        private string? _userEmail_TopBarView_TextBlock;

        [ObservableProperty]
        private string? _userMembershipType_TopBarView_TextBlock;

        public event Action? NavigateToDashboardRequested;
        public event Action? NavigateToAccountRequested;
        public event Action? NavigateToBudegtRequested;
        public event Action? NavigateToTransactionsRequested;
        public event Action? NavigateToCurrenciesRequested;
        public event Action? NavigateToDebtRequested;
        public event Action? NavigateToReportsRequested;
        public event Action? NavigateToFinBotRequested;
        public event Action? NavigateToFeedbackRequested;
        public event Action? NavigateToSettingsRequested;
        public event Action? NavigateToNotificationRequested;

        [RelayCommand]
        private void NavigateToDashboard_TopBarView_Button()
        {
            NavigateToDashboardRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToAccount_TopBarView_Button()
        {
            NavigateToAccountRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToBudget_TopBarView_Button()
        {
            NavigateToBudegtRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToTransactions_TopBarView_Button()
        {
            NavigateToTransactionsRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToCurrencies_TopBarView_Button()
        {
            NavigateToCurrenciesRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToDebt_TopBarView_Button()
        {
            NavigateToDebtRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToReports_TopBarView_Button()
        {
            NavigateToReportsRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToFinBot_TopBarView_Button()
        {
            NavigateToFinBotRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToFeedback_TopBarView_Button()
        {
            NavigateToFeedbackRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToNotification_TopBarView_Button()
        {
            NavigateToNotificationRequested?.Invoke();
        }

        [RelayCommand]
        private void NavigateToSettings_TopBarView_Button()
        {
            NavigateToSettingsRequested?.Invoke();
        }
    }
}
