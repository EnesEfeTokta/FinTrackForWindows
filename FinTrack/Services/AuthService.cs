using FinTrack.Dtos;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                LoginRequestDto request = new LoginRequestDto { Email = email, Password = password };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/user/login", request);

                if (response.IsSuccessStatusCode)
                {
                    LoginResponseDto? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken) && !string.IsNullOrEmpty(loginResponse.RefreshToken))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

                        return loginResponse.AccessToken;
                    }
                    else
                    {
                        throw new Exception("Login response is null.");
                    }
                }
                else
                {
                    throw new Exception($"Login failed: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during login: {ex.Message}");
                return string.Empty;
            }
        }

        public void Logout()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> InitiateRegistrationAsnc(string userName, string email, string password)
        {
            try
            {
                RegisterRequestDto request = new RegisterRequestDto
                {
                    UserName = userName,
                    Email = email,
                    Password = password,
                    ProfilePicture = null
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/user/initiate-registration", request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during registration: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerifyOtpAndRegisterCodeAsync(string email, string code)
        {
            try
            {
                OtpVerifyCodeRequestDto request = new OtpVerifyCodeRequestDto
                {
                    Email = email,
                    Code = code
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/user/verify-otp-and-register", request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during OTP verification: {ex.Message}");
                return false;
            }
        }
    }
}
