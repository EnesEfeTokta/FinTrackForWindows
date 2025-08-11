using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Dtos.UserDtos;
using FinTrackForWindows.Models.User;
using FinTrackForWindows.Services.Api;

namespace FinTrackForWindows.Services.Users
{
    public class UserStore : IUserStore
    {
        private readonly IApiService _apiService;
        private UserModel? _currentUser;

        public UserModel? CurrentUser => _currentUser;
        public event Action? UserChanged;

        public UserStore(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task LoadCurrentUserAsync()
        {
            try
            {
                var userProfile = await _apiService.GetAsync<UserProfileDto>("user");

                if (userProfile != null)
                {
                    _currentUser = MapProfileDtoToUserModel(userProfile);
                    OnUserChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading current user: {ex.Message}");
                ClearCurrentUser();
            }
        }

        public void ClearCurrentUser()
        {
            if (_currentUser != null)
            {
                _currentUser = null;
                OnUserChanged();
            }
        }

        public async Task<bool> UpdateProfilePictureAsync(UpdateProfilePictureDto pictureDto)
        {
            if (CurrentUser == null) return false;

            try
            {
                await _apiService.PostAsync<object>("usersettings/update-profile-picture", pictureDto);

                CurrentUser.ProfilePictureUrl = pictureDto.ProfilePictureUrl;
                OnUserChanged();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile picture: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserNameAsync(UpdateUserNameDto nameDto)
        {
            if (CurrentUser == null) return false;

            try
            {
                await _apiService.PostAsync<object>("usersettings/update-username", nameDto);

                CurrentUser.UserName = $"{nameDto.FirstName.Trim()}_{nameDto.LastName.Trim()}";
                OnUserChanged();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating username: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserPasswordAsync(UpdateUserPasswordDto passwordDto)
        {
            try
            {
                await _apiService.PostAsync<object>("usersettings/update-password", passwordDto);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                return false;
            }
        }

        public async Task RequestEmailChangeOtpAsync()
        {
            try
            {
                await _apiService.PostAsync<object>("usersettings/request-email-change", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error requesting email change OTP: {ex.Message}");
            }
        }

        public async Task<bool> UpdateUserEmailAsync(UpdateUserEmailDto emailDto)
        {
            if (CurrentUser == null) return false;

            try
            {
                await _apiService.PostAsync<object>("usersettings/confirm-email-change", emailDto);

                CurrentUser.Email = emailDto.NewEmail;
                OnUserChanged();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming email change: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateNotificationSettingsAsync(UserNotificationSettingsUpdateDto settingsDto)
        {
            if (CurrentUser == null) return false;

            try
            {
                await _apiService.PostAsync<object>("usersettings/user-notificationettings", settingsDto);

                CurrentUser.SpendingLimitWarning = settingsDto.SpendingLimitWarning;
                CurrentUser.ExpectedBillReminder = settingsDto.ExpectedBillReminder;
                CurrentUser.WeeklySpendingSummary = settingsDto.WeeklySpendingSummary;
                CurrentUser.NewFeaturesAndAnnouncements = settingsDto.NewFeaturesAndAnnouncements;
                CurrentUser.EnableDesktopNotifications = settingsDto.EnableDesktopNotifications;
                OnUserChanged();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification settings: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAppSettingsAsync(UserAppSettingsUpdateDto settingsDto)
        {
            if (CurrentUser == null) return false;

            try
            {
                await _apiService.PostAsync<object>("usersettings/app-settings", settingsDto);

                CurrentUser.Thema = settingsDto.Appearance;
                CurrentUser.Currency = settingsDto.Currency;
                OnUserChanged();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating app settings: {ex.Message}");
                return false;
            }
        }

        private void OnUserChanged()
        {
            UserChanged?.Invoke();
        }

        private UserModel MapProfileDtoToUserModel(UserProfileDto dto)
        {
            return new UserModel
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Email = dto.Email,
                ProfilePictureUrl = dto.ProfilePictureUrl,
                CreatedAtUtc = dto.CreatedAtUtc,
                CurrentMembershipPlanId = dto.CurrentMembershipPlanId,
                CurrentMembershipPlanType = dto.CurrentMembershipPlanType,
                MembershipStartDateUtc = dto.MembershipStartDateUtc,
                MembershipExpirationDateUtc = dto.MembershipExpirationDateUtc,
                Thema = dto.Thema,
                Language = dto.Language,
                Currency = dto.Currency,
                SpendingLimitWarning = dto.SpendingLimitWarning,
                ExpectedBillReminder = dto.ExpectedBillReminder,
                WeeklySpendingSummary = dto.WeeklySpendingSummary,
                NewFeaturesAndAnnouncements = dto.NewFeaturesAndAnnouncements,
                EnableDesktopNotifications = dto.EnableDesktopNotifications,
                CurrentAccounts = dto.CurrentAccounts,
                CurrentBudgets = dto.CurrentBudgets,
                CurrentTransactions = dto.CurrentTransactions,
                CurrentBudgetsCategories = dto.CurrentBudgetsCategories,
                CurrentTransactionsCategories = dto.CurrentTransactionsCategories,
                CurrentLenderDebts = dto.CurrentLenderDebts,
                CurrentBorrowerDebts = dto.CurrentBorrowerDebts,
                CurrentNotifications = dto.CurrentNotifications,
                CurrentFeedbacks = dto.CurrentFeedbacks,
                CurrentVideos = dto.CurrentVideos
            };
        }
    }
}