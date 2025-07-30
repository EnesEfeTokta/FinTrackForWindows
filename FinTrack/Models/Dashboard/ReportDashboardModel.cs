using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.Models.Dashboard
{
    public partial class ReportDashboardModel : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;

        public ObservableCollection<DocumentFormat> Formats { get; set; }

        private readonly ILogger<ReportDashboardModel> _logger;

        public ReportDashboardModel(ILogger<ReportDashboardModel> logger)
        {
            _logger = logger;

            Formats = new ObservableCollection<DocumentFormat>();

            foreach (DocumentFormat exportFormat in Enum.GetValues(typeof(DocumentFormat)))
            {
                Formats.Add(exportFormat);
            }
        }

        [RelayCommand]
        private void Generate(DocumentFormat format)
        {

            _logger.LogInformation("Rapor oluşturuluyor -> Rapor Adı: {ReportName}, Format: {Format}", this.Name, format);
            MessageBox.Show($"Rapor oluşturuldu:\nAd: {this.Name}\nFormat: {format}", "Rapor Oluşturma", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
