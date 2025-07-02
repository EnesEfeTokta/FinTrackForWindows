using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinTrack.Dtos;
using FinTrack.Messages;
using FinTrack.Services.Api;
using Microsoft.Extensions.Logging;

namespace FinTrack.ViewModels
{
    public partial class TopBarViewModel : ObservableObject, IRecipient<LoginSuccessMessage>
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

        private readonly ILogger<TopBarViewModel> _logger;
        private readonly IApiService _apiService;
        private readonly IMessenger _messenger;

        public TopBarViewModel(ILogger<TopBarViewModel> logger, IApiService apiService, IMessenger messenger)
        {
            _logger = logger;
            _apiService = apiService;
            _messenger = messenger;

            _messenger.Register<LoginSuccessMessage>(this);
        }

        public void Receive(LoginSuccessMessage message)
        {
            _logger.LogInformation("Login başarılı, TopBarViewModel profil bilgilerini yüklüyor.");
            _ = LoadProfile();
        }

        private async Task LoadProfile()
        {
            var userProfile = await _apiService.GetAsync<UserProfileDto>("user");

            if (userProfile != null)
            {
                UserAvatar_TopBarView_Image = userProfile.ProfilePicture;
                UserFullName_TopBarView_TextBlock = userProfile.UserName;
                UserEmail_TopBarView_TextBlock = userProfile.Email;
                UserMembershipType_TopBarView_TextBlock = userProfile.MembershipType;
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
