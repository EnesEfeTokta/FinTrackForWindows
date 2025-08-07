using FinTrackForWindows.Dtos;
using FinTrackForWindows.Models.User;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.Services.Users
{
    public class UserStore : IUserStore
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserStore> _logger;
        private UserModel? _currentUser;

        public UserModel? CurrentUser
        {
            get => _currentUser;
            private set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    UserChanged?.Invoke();
                }
            }
        }

        public event Action? UserChanged;

        public UserStore(IApiService apiService, ILogger<UserStore> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task LoadCurrentUserAsync()
        {
            _logger.LogInformation("Mevcut kullanıcı bilgileri yükleniyor...");
            try
            {
                var userDto = await _apiService.GetAsync<UserProfileDto>("User");
                var user = new UserModel
                {
                    Id = userDto.Id,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    ProfilePictureUrl = userDto.ProfilePicture ?? string.Empty,
                    MembershipPlan = userDto.MembershipType,
                };
                if (userDto != null)
                {
                    CurrentUser = user;
                    _logger.LogInformation("Kullanıcı bilgileri başarıyla yüklendi: {UserName}", userDto.UserName);
                }
                else
                {
                    _logger.LogWarning("API'den kullanıcı bilgileri alınamadı veya kullanıcı bulunamadı.");
                    ClearCurrentUser();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı bilgileri yüklenirken bir hata oluştu.");
                ClearCurrentUser();
            }
        }

        public async Task<bool> UpdateUserAsync(UserModel updatedUserData)
        {
            //_logger.LogInformation("Kullanıcı bilgileri güncelleniyor: {UserName}", updatedUserData.UserName);
            //try
            //{
            //    var updatedUserFromApi = await _apiService.PutAsync<UserModel>("User", updatedUserData);

            //    if (updatedUserFromApi != null)
            //    {
            //        CurrentUser = updatedUserFromApi;
            //        _logger.LogInformation("Kullanıcı bilgileri başarıyla güncellendi.");
            //        return true;
            //    }

            //    _logger.LogWarning("Kullanıcı güncelleme isteği API tarafından başarısız oldu veya geçersiz veri döndü.");
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Kullanıcı bilgileri güncellenirken bir hata oluştu.");
            //    return false;
            //}
            await Task.Delay(1000);
            return false;
        }

        public void ClearCurrentUser()
        {
            if (CurrentUser != null)
            {
                _logger.LogInformation("Mevcut kullanıcı bilgileri temizleniyor.");
                CurrentUser = null;
            }
        }
    }
}
