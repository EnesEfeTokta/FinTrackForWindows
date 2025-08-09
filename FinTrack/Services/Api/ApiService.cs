using FinTrackForWindows.Core;
using FinTrackForWindows.Dtos.ReportDtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            _baseUrl = "http://localhost:8090/";
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
            _logger.LogInformation("Initiating DELETE request: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("DELETE request failed: endpoint is null or empty. Endpoint: {Endpoint}", endpoint);
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }

            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

                _logger.LogInformation("DELETE request succeeded: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during DELETE request. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DELETE request. Endpoint: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            _logger.LogInformation("Initiating GET request: {Endpoint}", endpoint);
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var strean = await response.Content.ReadAsStreamAsync();
                var result = await _httpClient.GetFromJsonAsync<T>(endpoint, _jsonSerializerOptions);

                _logger.LogInformation("GET request succeeded: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during GET request. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error during GET request. Endpoint: {Endpoint}", endpoint);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during GET request. Endpoint: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<List<T>?> GetsAsync<T>(string endpoint)
        {
            _logger.LogInformation("Initiating GET (list) request: {Endpoint}", endpoint);
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var result = await _httpClient.GetFromJsonAsync<List<T>>(endpoint, _jsonSerializerOptions);

                _logger.LogInformation("GET (list) request succeeded: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during GET (list) request. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(List<T>);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error during GET (list) request. Endpoint: {Endpoint}", endpoint);
                return default(List<T>);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during GET (list) request. Endpoint: {Endpoint}", endpoint);
                return default(List<T>);
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            _logger.LogInformation("Initiating POST request: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("POST request failed: endpoint is null or empty. Endpoint: {Endpoint}", endpoint);
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (data == null)
            {
                _logger.LogError("POST request failed: data is null. Endpoint: {Endpoint}", endpoint);
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

                _logger.LogInformation("POST request succeeded: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during POST request. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during POST request. Endpoint: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            _logger.LogInformation("Initiating PUT request: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("PUT request failed: endpoint is null or empty. Endpoint: {Endpoint}", endpoint);
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (data == null)
            {
                _logger.LogError("PUT request failed: data is null. Endpoint: {Endpoint}", endpoint);
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

                _logger.LogInformation("PUT request succeeded: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during PUT request. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during PUT request. Endpoint: {Endpoint}", endpoint);
                return default(T);
            }
        }

        public async Task<bool> CreateCategoryAsync(string endpoint, object payload)
        {
            _logger.LogInformation("Initiating category creation (POST) request: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("Category creation request failed: endpoint is null or empty.");
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (payload == null)
            {
                _logger.LogError("Category creation request failed: payload is null.");
                throw new ArgumentNullException(nameof(payload));
            }

            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload, _jsonSerializerOptions);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<bool>(_jsonSerializerOptions);

                _logger.LogInformation("Category creation (POST) request succeeded: {Endpoint}", endpoint);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during category creation. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return false;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error during category creation. Ensure the API returns 'true' or 'false'. Endpoint: {Endpoint}", endpoint);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during category creation. Endpoint: {Endpoint}", endpoint);
                return false;
            }
        }

        public async Task<bool> UploadFileAsync(string endpoint, string filePath)
        {
            _logger.LogInformation("Initiating file upload request: {Endpoint}, File: {FilePath}", endpoint, filePath);
            if (!File.Exists(filePath))
            {
                _logger.LogError("File upload failed: file not found. File: {FilePath}", filePath);
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
                        _logger.LogInformation("File uploaded successfully: {Endpoint}", endpoint);
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("HTTP error during file upload. Status Code: {StatusCode}. Error: {Error}", response.StatusCode, errorContent);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during file upload. Endpoint: {Endpoint}", endpoint);
                return false;
            }
        }

        public async Task<(byte[] FileBytes, string FileName)?> PostAndDownloadReportAsync<T>(string endpoint, T payload)
        {
            _logger.LogInformation("Initiating report download (POST) request: {Endpoint}", endpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogError("Report download request failed: endpoint is null or empty.");
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }
            if (payload == null)
            {
                _logger.LogError("Report download request failed: payload is null.");
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
                        _logger.LogWarning("Content-Disposition header not found. Using default file name: {FileName}", fileName);
                    }

                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                    _logger.LogInformation("Report downloaded successfully: {FileName}, Size: {Size} bytes", fileName, fileBytes.Length);
                    return (fileBytes, fileName);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("HTTP error during report download. Status Code: {StatusCode}. Error: {Error}", response.StatusCode, errorContent);

                    throw new HttpRequestException($"Failed to download report from API. Server responded with status code '{response.StatusCode}'. Details: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during report download. Endpoint: {Endpoint}", endpoint);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during report download. Endpoint: {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<(Stream? Stream, string? ContentType, string? FileName)> StreamFileAsync(string endpoint)
        {
            _logger.LogInformation("Initiating file streaming request: {Endpoint}", endpoint);
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                var contentType = response.Content.Headers.ContentType?.ToString();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"');

                _logger.LogInformation("File stream retrieved successfully from {Endpoint}", endpoint);
                return (stream, contentType, fileName);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during file streaming. Endpoint: {Endpoint}. Status Code: {StatusCode}", endpoint, ex.StatusCode);
                return (null, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during file streaming. Endpoint: {Endpoint}", endpoint);
                return (null, null, null);
            }
        }
    }
}
