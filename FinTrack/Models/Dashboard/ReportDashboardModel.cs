using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.ReportDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Helpers;
using FinTrackForWindows.Services.Reports;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.Models.Dashboard
{
    public partial class ReportDashboardModel : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;

        public ReportType Type { get; set; }

        public ObservableCollection<DocumentFormat> Formats { get; set; }

        private readonly ILogger<ReportDashboardModel> _logger;

        private readonly IReportStore _reportStore;

        public ReportDashboardModel(ILogger<ReportDashboardModel> logger, IReportStore reportStore)
        {
            _logger = logger;
            _reportStore = reportStore;

            Formats = new ObservableCollection<DocumentFormat>();

            foreach (DocumentFormat exportFormat in Enum.GetValues(typeof(DocumentFormat)))
            {
                Formats.Add(exportFormat);
            }
        }

        [RelayCommand]
        private async Task Generate(DocumentFormat format)
        {
            _logger.LogInformation("Hızlı rapor oluşturuluyor -> Rapor Adı: {ReportName}, Format: {Format}", this.Name, format);

            var reportRequest = new ReportRequestDto
            {
                ReportType = Type,
                ExportFormat = format,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 30),
                MinBalance = null,
                MaxBalance = null,
                IsIncomeSelected = false,
                IsExpenseSelected = false,
                SelectedSortingCriterion = null,
                SelectedAccountIds = null,
                SelectedCategoryIds = null,
            };

            string? savedPath = await _reportStore.CreateAndSaveReportAsync(reportRequest);

            if (!string.IsNullOrEmpty(savedPath))
            {
                MessageBox.Show($"Report created successfully and saved to '{savedPath}'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                FileSaver.OpenContainingFolder(savedPath);
            }
            else
            {
                _logger.LogWarning("ReportStore reported failure in report creation (no data or server error).");
                MessageBox.Show("Could not create report. No data found for the specified criteria or a server error occurred.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
