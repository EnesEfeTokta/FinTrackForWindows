using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Models.User;

namespace FinTrackForWindows.Services.Users
{
    public interface IUserStore
    {
        UserModel? CurrentUser { get; }
        event Action? UserChanged;
        Task LoadCurrentUserAsync();

        Task<bool> UpdateProfilePictureAsync(UpdateProfilePictureDto pictureDto);
        Task<bool> UpdateUserNameAsync(UpdateUserNameDto nameDto);
        Task<bool> UpdateUserPasswordAsync(UpdateUserPasswordDto passwordDto);
        Task RequestEmailChangeOtpAsync();
        Task<bool> UpdateUserEmailAsync(UpdateUserEmailDto emailDto);

        Task<bool> UpdateAppSettingsAsync(UserAppSettingsUpdateDto settingsDto);
        Task<bool> UpdateNotificationSettingsAsync(UserNotificationSettingsUpdateDto settingsDto);

        void ClearCurrentUser();
    }
}
