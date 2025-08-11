using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Services.StoresRefresh;
using Microsoft.Extensions.Logging;
using System.Windows;


namespace FinTrackForWindows.ViewModels
{
    public partial class BottomBarViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string company = string.Empty;

        [ObservableProperty]
        private string version = string.Empty;

        [ObservableProperty]
        private string lastSyncStatus = "Awaiting first synchronization...";

        private readonly ILogger<BottomBarViewModel> _logger;
        private readonly IStoresRefresh _storesRefresh;
        private readonly Timer _refreshTimer;

        public BottomBarViewModel(ILogger<BottomBarViewModel> logger, IStoresRefresh storesRefresh)
        {
            _logger = logger;
            _storesRefresh = storesRefresh;

            int year = DateTime.Now.Year;
            Company = $"© {year} FinTrack Inc.";
            Version = "v1.0.0";

            _storesRefresh.RefreshStarted += OnRefreshStarted;
            _storesRefresh.RefreshCompleted += OnRefreshCompleted;

            _refreshTimer = new Timer(
                callback: RefreshStores,
                state: null,
                dueTime: TimeSpan.FromSeconds(5),
                period: TimeSpan.FromMinutes(5));
        }

        private void OnRefreshStarted()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LastSyncStatus = "Synchronization in progress...";
            });
        }

        private void OnRefreshCompleted(bool success)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (success)
                {
                    LastSyncStatus = $"Last Synchronization: Successful at {DateTime.Now:HH:mm:ss}";
                    _logger.LogInformation("Synchronization completed successfully.");
                }
                else
                {
                    LastSyncStatus = $"Last Synchronization: Failed at {DateTime.Now:HH:mm:ss}";
                    _logger.LogWarning("Synchronization failed.");
                }
            });
        }

        private async void RefreshStores(object? state)
        {
            _logger.LogInformation("Periodic synchronization triggered.");
            await _storesRefresh.RefreshAllStoresAsync();
        }

        [RelayCommand]
        private void AddNewTransaction()
        {
            _logger.LogInformation("Adding new transaction...");
            MessageBox.Show("The add new transaction feature is not implemented yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Dispose()
        {
            _refreshTimer?.Dispose();
            _storesRefresh.RefreshStarted -= OnRefreshStarted;
            _storesRefresh.RefreshCompleted -= OnRefreshCompleted;
            GC.SuppressFinalize(this);
        }
    }
}
