using FinTrackForWindows.Dtos.MembershipDtos;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.Services.Memberships
{
    public interface IMembershipStore
    {
        UserMembershipDto CurrentUserMembership { get; }
        ReadOnlyObservableCollection<PlanFeatureDto> AvailablePlans { get; }
        ReadOnlyObservableCollection<UserMembershipDto> MembershipHistory { get; }
        Task LoadAllMembershipDataAsync();
        Task<string> SelectPlanAsync(int planId, bool autoRenew);
        Task CancelCurrentSubscriptionAsync();
        event Action CurrentUserMembershipChanged;
    }
}
