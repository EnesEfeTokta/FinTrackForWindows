namespace FinTrackForWindows.Services.StoresRefresh
{
    public interface IStoresRefresh
    {
        event Action? RefreshStarted;
        event Action<bool>? RefreshCompleted;
        Task<bool> RefreshAllStoresAsync();
    }
}
