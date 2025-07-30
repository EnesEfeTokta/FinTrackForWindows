// ViewModels/ReportsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos;
using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Dtos.CategoryDtos;
using FinTrackForWindows.Dtos.ReportDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Helpers;
using FinTrackForWindows.Models.Report;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    // partial class olduğundan emin olun
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly ILogger<ReportsViewModel> _logger;
        private readonly IApiService _apiService;

        public ObservableCollection<ReportType> AvailableReportTypes { get; }
        public ObservableCollection<SelectableOptionReport> AvailableAccounts { get; }
        public ObservableCollection<SelectableOptionReport> AvailableCategories { get; }
        public ObservableCollection<string> SortingCriteria { get; }
        public ObservableCollection<DocumentFormat> AvailableDocumentFormats { get; }

        // --- DOĞRU KULLANIM: Sadece [ObservableProperty] ile private alanları tanımlayın ---
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
        private string selectedSortingCriterion;

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

        public ReportsViewModel(ILogger<ReportsViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            AvailableReportTypes = new ObservableCollection<ReportType>(Enum.GetValues(typeof(ReportType)).Cast<ReportType>());
            AvailableAccounts = new ObservableCollection<SelectableOptionReport>();
            AvailableCategories = new ObservableCollection<SelectableOptionReport>();
            SortingCriteria = new ObservableCollection<string>();
            AvailableDocumentFormats = new ObservableCollection<DocumentFormat>(Enum.GetValues(typeof(DocumentFormat)).Cast<DocumentFormat>());

            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            IsBusy = true;
            try
            {
                var accountsFromApi = await _apiService.GetAsync<List<AccountDto>>("Account");
                var categoriesFromApi = await _apiService.GetAsync<List<CategoryDto>>("categories");

                AvailableAccounts.Clear();
                // "All" seçeneğini artık ViewModel'de kontrol edeceğimiz için eklemeye gerek yok.
                if (accountsFromApi != null)
                {
                    foreach (var acc in accountsFromApi)
                    {
                        AvailableAccounts.Add(new SelectableOptionReport(acc.Id, acc.Name));
                    }
                }

                AvailableCategories.Clear();
                if (categoriesFromApi != null)
                {
                    foreach (var cat in categoriesFromApi)
                    {
                        AvailableCategories.Add(new SelectableOptionReport(cat.Id, cat.Name));
                    }
                }

                SortingCriteria.Clear();
                SortingCriteria.Add("By Date (Newest to Oldest)");
                SortingCriteria.Add("By Date (Oldest to Newest)");
                SortingCriteria.Add("By Amount (Highest to Lowest)");
                SortingCriteria.Add("By Amount (Lowest to Highest)");

                SelectedReportType = AvailableReportTypes.FirstOrDefault();
                SelectedSortingCriterion = SortingCriteria.FirstOrDefault();
                SelectedDocumentFormat = AvailableDocumentFormats.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load initial data for reports.");
                MessageBox.Show("Could not load account and category data. Please check your internet connection.", "Connection Error");
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

                _logger.LogInformation("Sending report creation request. Type: {ReportType}", reportRequest.ReportType);

                var result = await _apiService.PostAndDownloadReportAsync("Reports/generate", reportRequest);

                if (result.HasValue && result.Value.FileBytes.Length > 0)
                {
                    var (fileBytes, fileName) = result.Value;
                    string savedPath = await FileSaver.SaveReportToDocumentsAsync(fileBytes, fileName);
                    _logger.LogInformation("Report saved successfully: {Path}", savedPath);
                    MessageBox.Show($"Report created successfully and saved to '{savedPath}'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    FileSaver.OpenContainingFolder(savedPath);
                }
                else
                {
                    _logger.LogWarning("No file data received from API or no data found for the report.");
                    MessageBox.Show("Could not create report. No data found for the specified criteria or a server error occurred.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the report.");
                MessageBox.Show($"An unexpected error occurred:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}