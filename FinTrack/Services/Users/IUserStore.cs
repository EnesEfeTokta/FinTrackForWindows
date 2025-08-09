using FinTrackForWindows.Models.User;

namespace FinTrackForWindows.Services.Users
{
    public interface IUserStore
    {
        UserModel? CurrentUser { get; }
        event Action? UserChanged;
        Task LoadCurrentUserAsync();
        Task<bool> UpdateUserAsync(UserModel updatedUserData);

        void ClearCurrentUser();
    }
}
