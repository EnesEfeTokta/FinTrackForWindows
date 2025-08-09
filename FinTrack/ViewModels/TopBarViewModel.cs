using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.Services.Users;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class TopBarViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userAvatar = "/Assets/Images/Icons/user-red.png";

        [ObservableProperty]
        private string userFullName = "User Full Name";

        [ObservableProperty]
        private string userEmail = "user@example.com";

        [ObservableProperty]
        private string userMembershipType = "Free";

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

        private readonly ILogger<TopBarViewModel> _logger;
        private readonly IApiService _apiService;

        private readonly IUserStore _userStore;

        public TopBarViewModel(ILogger<TopBarViewModel> logger, IApiService apiService, IUserStore userStore)
        {
            _logger = logger;
            _apiService = apiService;
            _userStore = userStore;

            if (SessionManager.IsLoggedIn)
            {
                _logger.LogInformation("User is already logged in. Loading profile information in TopBarViewModel.");
                _ = LoadProfile();
            }
        }

        private async Task LoadProfile()
        {
            if (!SessionManager.IsLoggedIn)
            {
                _logger.LogWarning("User session is not active. Unable to load profile information.");
                return;
            }

            await _userStore.LoadCurrentUserAsync();

            var userProfile = _userStore.CurrentUser;

            if (userProfile != null)
            {
                UserAvatar = userProfile.ProfilePictureUrl;
                UserFullName = userProfile.UserName;
                UserEmail = userProfile.Email;
                UserMembershipType = $"{userProfile.MembershipPlan} Member";
                _logger.LogInformation("User profile loaded successfully. Name: {UserName}, Email: {Email}, Membership Type: {MembershipType}",
                    userProfile.UserName, userProfile.Email, userProfile.MembershipPlan);
            }
            else
            {
                _logger.LogWarning("Failed to load user profile.");
            }
        }

        [RelayCommand]
        private void NavigateToDashboard_TopBarView_Button()
        {
            NavigateToDashboardRequested?.Invoke();
            _logger.LogInformation("Navigated to Dashboard panel.");
        }

        [RelayCommand]
        private void NavigateToAccount_TopBarView_Button()
        {
            NavigateToAccountRequested?.Invoke();
            _logger.LogInformation("Navigated to Account panel.");
        }

        [RelayCommand]
        private void NavigateToBudget_TopBarView_Button()
        {
            NavigateToBudegtRequested?.Invoke();
            _logger.LogInformation("Navigated to Budget panel.");
        }

        [RelayCommand]
        private void NavigateToTransactions_TopBarView_Button()
        {
            NavigateToTransactionsRequested?.Invoke();
            _logger.LogInformation("Navigated to Transactions panel.");
        }

        [RelayCommand]
        private void NavigateToCurrencies_TopBarView_Button()
        {
            NavigateToCurrenciesRequested?.Invoke();
            _logger.LogInformation("Navigated to Currencies panel.");
        }

        [RelayCommand]
        private void NavigateToDebt_TopBarView_Button()
        {
            NavigateToDebtRequested?.Invoke();
            _logger.LogInformation("Navigated to Debt panel.");
        }

        [RelayCommand]
        private void NavigateToReports_TopBarView_Button()
        {
            NavigateToReportsRequested?.Invoke();
            _logger.LogInformation("Navigated to Reports panel.");
        }

        [RelayCommand]
        private void NavigateToFinBot_TopBarView_Button()
        {
            NavigateToFinBotRequested?.Invoke();
            _logger.LogInformation("Navigated to FinBot panel.");
        }

        [RelayCommand]
        private void NavigateToFeedback_TopBarView_Button()
        {
            NavigateToFeedbackRequested?.Invoke();
            _logger.LogInformation("Navigated to Feedback panel.");
        }

        [RelayCommand]
        private void NavigateToNotification_TopBarView_Button()
        {
            NavigateToNotificationRequested?.Invoke();
            _logger.LogInformation("Navigated to Notification panel.");
        }

        [RelayCommand]
        private void NavigateToSettings_TopBarView_Button()
        {
            NavigateToSettingsRequested?.Invoke();
            _logger.LogInformation("Navigated to Settings panel.");
        }
    }
}
