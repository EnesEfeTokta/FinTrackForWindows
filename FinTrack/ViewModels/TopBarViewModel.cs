using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Dtos;
using FinTrackForWindows.Services.Api;
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

        public TopBarViewModel(ILogger<TopBarViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            if (SessionManager.IsLoggedIn)
            {
                _logger.LogInformation("Kullanıcı zaten giriş yapmış. TopBarViewModel profil bilgilerini yüklüyor.");
                _ = LoadProfile();
            }
        }

        private async Task LoadProfile()
        {
            if (!SessionManager.IsLoggedIn)
            {
                _logger.LogWarning("Kullanıcı oturumu açık değil, profil bilgileri yüklenemedi.");
                return;
            }
            var userProfile = await _apiService.GetAsync<UserProfileDto>("user");

            if (userProfile != null)
            {
                UserAvatar = userProfile.ProfilePicture;
                UserFullName = userProfile.UserName;
                UserEmail = userProfile.Email;
                UserMembershipType = userProfile.MembershipType;
                _logger.LogInformation("Kullanıcı profili başarıyla yüklendi. Kullanıcı Adı: {UserName}, Email: {Email}, Üyelik Tipi: {MembershipType}",
                    userProfile.UserName, userProfile.Email, userProfile.MembershipType);
            }
            else
            {
                _logger.LogWarning("Kullanıcı profili yüklenemedi.");
            }
        }

        [RelayCommand]
        private void NavigateToDashboard_TopBarView_Button()
        {
            NavigateToDashboardRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Dashboard paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToAccount_TopBarView_Button()
        {
            NavigateToAccountRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Account paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToBudget_TopBarView_Button()
        {
            NavigateToBudegtRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Budget paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToTransactions_TopBarView_Button()
        {
            NavigateToTransactionsRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Transactions paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToCurrencies_TopBarView_Button()
        {
            NavigateToCurrenciesRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Currencies paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToDebt_TopBarView_Button()
        {
            NavigateToDebtRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Debt paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToReports_TopBarView_Button()
        {
            NavigateToReportsRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Reports paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToFinBot_TopBarView_Button()
        {
            NavigateToFinBotRequested?.Invoke();
            _logger.LogInformation("Kullanıcı FinBot paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToFeedback_TopBarView_Button()
        {
            NavigateToFeedbackRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Feedback paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToNotification_TopBarView_Button()
        {
            NavigateToNotificationRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Notification paneline geçti.");
        }

        [RelayCommand]
        private void NavigateToSettings_TopBarView_Button()
        {
            NavigateToSettingsRequested?.Invoke();
            _logger.LogInformation("Kullanıcı Settings paneline geçti.");
        }
    }
}
