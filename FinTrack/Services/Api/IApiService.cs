using System.IO;

namespace FinTrackForWindows.Services.Api
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task<T> DeleteAsync<T>(string endpoint);
        Task<bool> CreateCategoryAsync(string endpoint, object payload);
        Task<bool> UploadFileAsync(string endpoint, string filePath);
        Task<(byte[] FileBytes, string FileName)?> PostAndDownloadReportAsync<T>(string endpoint, T payload);
        Task<(Stream? Stream, string? ContentType, string? FileName)> StreamFileAsync(string endpoint);
    }
}
