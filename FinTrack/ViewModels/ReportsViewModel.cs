using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.ReportDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Helpers;
using FinTrackForWindows.Models.Report;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Reports;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;

namespace FinTrackForWindows.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly ILogger<ReportsViewModel> _logger;
        private readonly IReportStore _reportStore;
        private readonly IAppInNotificationService _notificationService;

        public ObservableCollection<ReportType> AvailableReportTypes { get; }
        public ObservableCollection<SelectableOptionReport> AvailableAccounts { get; }
        public ObservableCollection<SelectableOptionReport> AvailableCategories { get; }
        public ObservableCollection<string> SortingCriteria { get; }
        public ObservableCollection<DocumentFormat> AvailableDocumentFormats { get; }

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

        public ReportsViewModel(ILogger<ReportsViewModel> logger, IReportStore reportStore, IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _reportStore = reportStore;
            _notificationService = appInNotificationService;

            AvailableReportTypes = _reportStore.AvailableReportTypes;
            AvailableAccounts = _reportStore.AvailableAccounts;
            AvailableCategories = _reportStore.AvailableCategories;
            SortingCriteria = _reportStore.SortingCriteria;
            AvailableDocumentFormats = _reportStore.AvailableDocumentFormats;

            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            IsBusy = true;
            try
            {
                await _reportStore.LoadInitialDataAsync();

                SelectedReportType = AvailableReportTypes.FirstOrDefault();
                SelectedSortingCriterion = SortingCriteria?.FirstOrDefault();
                SelectedDocumentFormat = AvailableDocumentFormats.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load initial data for reports from the Store.");
                _notificationService.ShowError("Could not load report options. Please check your internet connection.");
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
                    SelectedAccountIds = AvailableAccounts
                        .Where(acc => acc.IsSelected)
                        .Select(acc => acc.Id)
                        .ToList(),
                    SelectedCategoryIds = AvailableCategories
                        .Where(cat => cat.IsSelected)
                        .Select(cat => cat.Id)
                        .ToList()
                };

                _logger.LogInformation("Delegating report creation to ReportStore.");

                string? savedPath = await _reportStore.CreateAndSaveReportAsync(reportRequest);

                if (!string.IsNullOrEmpty(savedPath))
                {
                    _notificationService.ShowSuccess($"Report created successfully and saved to '{savedPath}'.");
                    FileSaver.OpenContainingFolder(savedPath);
                }
                else
                {
                    _logger.LogWarning("ReportStore reported failure in report creation.");
                    _notificationService.ShowWarning("Could not create report. No data found for the specified criteria.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in ViewModel while creating the report.");
                _notificationService.ShowError($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}