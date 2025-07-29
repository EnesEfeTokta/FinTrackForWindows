namespace FinTrackForWindows.Services.Api
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task<T> DeleteAsync<T>(string endpoint);
        Task<bool> UploadFileAsync(string endpoint, string filePath);
    }
}
