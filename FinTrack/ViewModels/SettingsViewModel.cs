using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.MembershipDtos;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Memberships;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FinTrackForWindows.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ILogger<SettingsViewModel> _logger;
        private readonly IMembershipStore _membershipStore;

        private readonly ProfileSettingsContentViewModel _profileVM;
        private readonly SecuritySettingsContentViewModel _securityVM;
        private readonly NotificationSettingsContentViewModel _notificationsVM;
        private readonly AppSettingsContentViewModel _appVM;

        [ObservableProperty]
        private ObservableObject _currentPageViewModel;

        public IMembershipStore MembershipStore => _membershipStore;

        private readonly IAppInNotificationService _appInNotificationService;

        public SettingsViewModel(
            ILogger<SettingsViewModel> logger,
            IMembershipStore membershipStore,
            ProfileSettingsContentViewModel profileVM,
            SecuritySettingsContentViewModel securityVM,
            NotificationSettingsContentViewModel notificationsVM,
            AppSettingsContentViewModel appVM,
            IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _membershipStore = membershipStore;
            _profileVM = profileVM;
            _securityVM = securityVM;
            _notificationsVM = notificationsVM;
            _appVM = appVM;
            _appInNotificationService = appInNotificationService;

            _currentPageViewModel = _profileVM;

            _ = LoadInitialData();
        }

        private async Task LoadInitialData()
        {
            await _membershipStore.LoadAllMembershipDataAsync();
        }

        [RelayCommand]
        private void ChangePage(string pageName)
        {
            _logger.LogInformation($"Changing settings page to: {pageName}");
            CurrentPageViewModel = pageName switch
            {
                "Profile" => _profileVM,
                "Security" => _securityVM,
                "Notifications" => _notificationsVM,
                "App" => _appVM,
                _ => CurrentPageViewModel
            };
        }

        [RelayCommand]
        private async Task SelectPlan(PlanFeatureDto selectedPlan)
        {
            if (selectedPlan == null)
            {
                _logger.LogWarning("SelectPlan command executed with a null parameter.");
                return;
            }

            _logger.LogInformation($"Plan selection initiated for: {selectedPlan.Name} (ID: {selectedPlan.Id})");

            if (MembershipStore.CurrentUserMembership != null && MembershipStore.CurrentUserMembership.PlanId == selectedPlan.Id)
            {
                _logger.LogInformation($"User is already subscribed to the selected plan: {selectedPlan.Name}");
                _appInNotificationService.ShowInfo($"You are already subscribed to the {selectedPlan.Name} plan.");
                return;
            }

            string checkoutUrl = await _membershipStore.SelectPlanAsync(selectedPlan.Id, true);

            if (!string.IsNullOrEmpty(checkoutUrl))
            {
                _logger.LogInformation($"Redirecting user to checkout URL: {checkoutUrl}");
                try
                {
                    Process.Start(new ProcessStartInfo(checkoutUrl) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to open web browser for checkout.");
                    _appInNotificationService.ShowError($"Could not open the payment page. Please copy this link and open it manually:\n\n{checkoutUrl}");
                }
            }
            else
            {
                _logger.LogInformation("No checkout URL received, assuming successful subscription to a free plan or an API error occurred.");
                _appInNotificationService.ShowSuccess("Your subscription status has been updated. Please check your profile.");
            }
        }
    }
}