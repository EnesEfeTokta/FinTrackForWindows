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
            LastSyncStatus = "Son Senkronizasyon: Başarılı";
        }

        [RelayCommand]
        private void AddNewTransaction()
        {
            _logger.LogInformation("Yeni işlem ekleniyor...");
            MessageBox.Show("Yeni işlem ekleme özelliği henüz uygulanmadı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
