using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.ReportDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Helpers;
using FinTrackForWindows.Services.Reports;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly ILogger<ReportsViewModel> _logger;
        private readonly IReportStore _reportStore;

        public IReportStore ReportStore => _reportStore;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReportSummary))]
        private ReportType selectedReportType;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReportSummary))]
        private DateTime? startDate = new DateTime(DateTime.Now.Year, 1, 1);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReportSummary))]
        private DateTime? endDate = new DateTime(DateTime.Now.Year, 12, 31);

        [ObservableProperty]
        private decimal? minBalance;

        [ObservableProperty]
        private decimal? maxBalance;

        [ObservableProperty]
        private bool isIncomeSelected = true;

        [ObservableProperty]
        private bool isExpenseSelected = true;

        [ObservableProperty]
        private string? selectedSortingCriterion;

        [ObservableProperty]
        private DocumentFormat selectedDocumentFormat;

        [ObservableProperty]
        private bool isBusy;

        public string ReportSummary
        {
            get
            {
                var summary = new StringBuilder();
                summary.Append($"{SelectedReportType} Report, ");
                summary.Append($"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}. ");
                summary.Append("This report will be created based on the selected filters.");
                return summary.ToString();
            }
        }

        public ReportsViewModel(ILogger<ReportsViewModel> logger, IReportStore reportStore)
        {
            _logger = logger;
            _reportStore = reportStore;

            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            IsBusy = true;
            try
            {
                await _reportStore.LoadInitialDataAsync();

                SelectedReportType = _reportStore.AvailableReportTypes.FirstOrDefault();
                SelectedSortingCriterion = _reportStore.SortingCriteria?.FirstOrDefault();
                SelectedDocumentFormat = _reportStore.AvailableDocumentFormats.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load initial data for reports from the Store.");
                MessageBox.Show("Could not load report options. Please check your internet connection or restart the application.", "Connection Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void SelectDocumentFormat(DocumentFormat format)
        {
            SelectedDocumentFormat = format;
        }

        [RelayCommand]
        private async Task CreateReport()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var reportRequest = new ReportRequestDto
                {
                    ReportType = SelectedReportType,
                    ExportFormat = SelectedDocumentFormat,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    MinBalance = MinBalance,
                    MaxBalance = MaxBalance,
                    IsIncomeSelected = IsIncomeSelected,
                    IsExpenseSelected = IsExpenseSelected,
                    SelectedSortingCriterion = SelectedSortingCriterion,
                    SelectedAccountIds = _reportStore.AvailableAccounts
                        .Where(acc => acc.IsSelected)
                        .Select(acc => acc.Id)
                        .ToList(),
                    SelectedCategoryIds = _reportStore.AvailableCategories
                        .Where(cat => cat.IsSelected)
                        .Select(cat => cat.Id)
                        .ToList()
                };

                _logger.LogInformation("Delegating report creation to ReportStore.");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in ViewModel while creating the report.");
                MessageBox.Show($"An unexpected error occurred:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}