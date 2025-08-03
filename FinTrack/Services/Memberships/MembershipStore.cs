using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Dtos.MembershipDtos;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.Services.Memberships
{
    public partial class MembershipStore : ObservableObject, IMembershipStore
    {
        private readonly IApiService _apiService;
        private readonly ILogger<MembershipStore> _logger;

        [ObservableProperty]
        private UserMembershipDto _currentUserMembership;

        private readonly ObservableCollection<PlanFeatureDto> _availablePlans;
        public ReadOnlyObservableCollection<PlanFeatureDto> AvailablePlans { get; }

        private readonly ObservableCollection<UserMembershipDto> _membershipHistory;
        public ReadOnlyObservableCollection<UserMembershipDto> MembershipHistory { get; }

        public event Action CurrentUserMembershipChanged;

        public MembershipStore(IApiService apiService, ILogger<MembershipStore> logger)
        {
            _apiService = apiService;
            _logger = logger;

            _availablePlans = new ObservableCollection<PlanFeatureDto>();
            AvailablePlans = new ReadOnlyObservableCollection<PlanFeatureDto>(_availablePlans);

            _membershipHistory = new ObservableCollection<UserMembershipDto>();
            MembershipHistory = new ReadOnlyObservableCollection<UserMembershipDto>(_membershipHistory);
        }

        public async Task LoadAllMembershipDataAsync()
        {
            _logger.LogInformation("Loading all membership data from API...");
            try
            {
                Task<IEnumerable<PlanFeatureDto>> plansTask = _apiService.GetAsync<IEnumerable<PlanFeatureDto>>("Membership/plans");
                Task<UserMembershipDto> currentMembershipTask = _apiService.GetAsync<UserMembershipDto>("Membership/current");
                Task<IEnumerable<UserMembershipDto>> historyTask = _apiService.GetAsync<IEnumerable<UserMembershipDto>>("Membership/history");

                await Task.WhenAll(plansTask, currentMembershipTask, historyTask);

                _availablePlans.Clear();
                var plans = await plansTask;
                if (plans != null)
                {
                    foreach (var plan in plans.OrderBy(p => p.Price))
                    {
                        _availablePlans.Add(plan);
                    }
                }

                CurrentUserMembership = await currentMembershipTask;

                _membershipHistory.Clear();
                var history = await historyTask;
                if (history != null)
                {
                    foreach (var item in history.OrderByDescending(h => h.StartDate))
                    {
                        _membershipHistory.Add(item);
                    }
                }

                CurrentUserMembershipChanged?.Invoke();
                _logger.LogInformation("All membership data loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load all membership data.");
                CurrentUserMembership = null;
                _availablePlans.Clear();
                _membershipHistory.Clear();
                CurrentUserMembershipChanged?.Invoke();
            }
        }

        public async Task<string> SelectPlanAsync(int planId, bool autoRenew)
        {
            _logger.LogInformation($"Initiating plan selection for Plan ID: {planId}.");
            try
            {
                var request = new SubscriptionRequestDto { PlanId = planId, AutoRenew = autoRenew };

                var responseDto = await _apiService.PostAsync<CheckoutResponseDto>("Membership/create-checkout-session", request);

                await LoadAllMembershipDataAsync();

                return responseDto?.CheckoutUrl ?? "N/A";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while selecting plan ID: {PlanId}.", planId);
                return string.Empty;
            }
        }

        public async Task CancelCurrentSubscriptionAsync()
        {
            if (CurrentUserMembership == null)
            {
                _logger.LogWarning("Attempted to cancel a subscription, but no active subscription found.");
                return;
            }

            _logger.LogInformation($"Initiating cancellation for subscription ID: {CurrentUserMembership.Id}.");
            try
            {
                await _apiService.PostAsync<bool>($"Membership/{CurrentUserMembership.Id}/cancel", null);
                await LoadAllMembershipDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cancelling subscription ID: {SubscriptionId}.", CurrentUserMembership.Id);
            }
        }
    }
}