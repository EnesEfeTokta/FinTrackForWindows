using FinTrackForWindows.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinTrackForWindows.Dtos.ReportDtos;

namespace FinTrackForWindows.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConfiguration _configuration;


        public ApiService(ILogger<ApiService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _baseUrl = "http://localhost:5246/";
            //_baseUrl = _configuration["BaseServerUrl"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
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
                var result = await _httpClient.GetFromJsonAsync<T>(endpoint, _jsonSerializerOptions);

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

        public async Task<List<T>?> GetsAsync<T>(string endpoint)
        {
            _logger.LogInformation("GET (list) isteği başlatılıyor: {Endpoint}", endpoint);
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var result = await _httpClient.GetFromJsonAsync<List<T>>(endpoint, _jsonSerializerOptions);

                _logger.LogInformation("GET (list) isteği başarılı: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GET (list) isteği sırasında HTTP hatası oluştu: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(List<T>);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "GET (list) isteği sırasında JSON serileştirme hatası oluştu: {Endpoint}", endpoint);
                return default(List<T>);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET (list) isteği sırasında beklenmeyen bir hata oluştu: {Endpoint}", endpoint);
                return default(List<T>);
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

                var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonSerializerOptions);
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

        public async Task<bool> UploadFileAsync(string endpoint, string filePath)
        {
            _logger.LogInformation("Dosya yükleme isteği başlatılıyor: {Endpoint}, Dosya: {FilePath}", endpoint, filePath);
            if (!File.Exists(filePath))
            {
                _logger.LogError("Yüklenecek dosya bulunamadı: {FilePath}", filePath);
                return false;
            }

            try
            {
                AddAuthorizationHeader();

                using (var content = new MultipartFormDataContent())
                {
                    var fileBytes = await File.ReadAllBytesAsync(filePath);
                    var fileContent = new ByteArrayContent(fileBytes);

                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                    content.Add(fileContent, "file", Path.GetFileName(filePath));
                    var response = await _httpClient.PostAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Dosya başarıyla yüklendi: {Endpoint}", endpoint);
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Dosya yükleme sırasında HTTP hatası: {StatusCode} - {Error}", response.StatusCode, errorContent);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya yükleme sırasında genel bir hata oluştu: {Endpoint}", endpoint);
                return false;
            }
        }

        public async Task<(byte[] FileBytes, string FileName)?> PostAndDownloadReportAsync<T>(string endpoint, T payload)
        {
            _logger.LogInformation("Rapor indirme (POST) isteği başlatılıyor: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("Rapor indirme isteği sırasında endpoint boş veya null.");
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (payload == null)
            {
                _logger.LogError("Rapor indirme isteği sırasında gönderilecek veri (payload) null.");
                throw new ArgumentNullException(nameof(payload));
            }

            try
            {
                AddAuthorizationHeader();

                var jsonContent = JsonContent.Create(payload, options: _jsonSerializerOptions);

                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    string fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"');

                    if (string.IsNullOrEmpty(fileName))
                    {
                        var format = "bin";
                        if (payload is ReportRequestDto dto)
                        {
                            format = dto.ExportFormat.ToString().ToLower();
                        }
                        fileName = $"report_{DateTime.Now:yyyyMMddHHmmss}.{format}";
                        _logger.LogWarning("Content-Disposition başlığı bulunamadı. Varsayılan dosya adı kullanılıyor: {FileName}", fileName);
                    }

                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                    _logger.LogInformation("Rapor başarıyla indirildi: {FileName}, Boyut: {Size} bytes", fileName, fileBytes.Length);
                    return (fileBytes, fileName);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Rapor indirme sırasında HTTP hatası: {StatusCode} - {Error}", response.StatusCode, errorContent);

                    throw new HttpRequestException($"API'den rapor alınamadı. Sunucu '{response.StatusCode}' durum kodu ile cevap verdi. Detay: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Rapor indirme sırasında HTTP isteği hatası: {Endpoint}", endpoint);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor indirme sırasında genel bir hata oluştu: {Endpoint}", endpoint);
                throw;
            }
        }
    }
}
