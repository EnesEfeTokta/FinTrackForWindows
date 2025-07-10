using FinTrack.Core;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FinTrack.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiService(ILogger<ApiService> logger)
        {
            _baseUrl = "http://localhost:5000/api/";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private void AddAuthorizationHeader()
        {
            string token = SessionManager.CurrentToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T?> DeleteAsync<T>(string endpoint)
        {
            _logger.LogInformation("DELETE isteği başlatılıyor: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("DELETE isteği sırasında endpoint boş veya null: {Endpoint}", endpoint);
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }

            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

                _logger.LogInformation("DELETE isteği başarılı: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "DELETE isteği sırasında HTTP hatası oluştu: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DELETE isteği sırasında genel bir hata oluştu: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            _logger.LogInformation("GET isteği başlatılıyor: {Endpoint}", endpoint);
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var strean = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(strean, _jsonSerializerOptions);

                _logger.LogInformation("GET isteği başarılı: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GET isteği sırasında HTTP hatası oluştu: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "GET isteği sırasında JSON serileştirme hatası oluştu: {Endpoint}", endpoint);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET isteği sırasında beklenmeyen bir hata oluştu: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            _logger.LogInformation("POST isteği başlatılıyor: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("POST isteği sırasında endpoint boş veya null: {Endpoint}", endpoint);
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (data == null)
            {
                _logger.LogError("POST isteği sırasında veri null: {Endpoint}", endpoint);
                throw new ArgumentNullException(nameof(data), "Data cannot be null");
            }

            try
            {
                AddAuthorizationHeader();

                var jsonPayload = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

                _logger.LogInformation("POST isteği başarılı: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "POST isteği sırasında HTTP hatası oluştu: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST isteği sırasında genel bir hata oluştu: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            _logger.LogInformation("PUT isteği başlatılıyor: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("PUT isteği sırasında endpoint boş veya null: {Endpoint}", endpoint);
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (data == null)
            {
                _logger.LogError("PUT isteği sırasında veri null: {Endpoint}", endpoint);
                throw new ArgumentNullException(nameof(data), "Data cannot be null");
            }

            try
            {
                AddAuthorizationHeader();

                var jsonPayload = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

                _logger.LogInformation("PUT isteği başarılı: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "PUT isteği sırasında HTTP hatası oluştu: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PUT isteği sırasında genel bir hata oluştu: {Endpoint}", endpoint);
                return default(T);
            }
        }
    }
}
