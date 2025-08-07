using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows;


namespace FinTrackForWindows.ViewModels
{
    public partial class BottomBarViewModel : ObservableObject
    {
        [ObservableProperty]
        private string company = string.Empty;

        [ObservableProperty]
        private string version = string.Empty;

        [ObservableProperty]
        private string lastSyncStatus = string.Empty;

        private readonly ILogger<BottomBarViewModel> _logger;

        public BottomBarViewModel(ILogger<BottomBarViewModel> logger)
        {
            _logger = logger;

            int year = DateTime.Now.Year;
            Company = $"© {year} FinTrack Inc.";

            Version = "v1.0.0";
            LastSyncStatus = "Last Synchronization: Successful";
        }

        [RelayCommand]
        private void AddNewTransaction()
        {
            _logger.LogInformation("Adding new transaction...");
            MessageBox.Show("The add new transaction feature is not implemented yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
