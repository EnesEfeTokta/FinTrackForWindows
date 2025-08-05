using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Dtos.CategoryDtos;
using FinTrackForWindows.Dtos.ReportDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Helpers;
using FinTrackForWindows.Models.Report;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.Services.Reports
{
    public partial class ReportStore : ObservableObject, IReportStore
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ReportStore> _logger;

        public ObservableCollection<ReportType> AvailableReportTypes { get; }
        public ObservableCollection<SelectableOptionReport> AvailableAccounts { get; }
        public ObservableCollection<SelectableOptionReport> AvailableCategories { get; }
        public ObservableCollection<string> SortingCriteria { get; }
        public ObservableCollection<DocumentFormat> AvailableDocumentFormats { get; }

        [ObservableProperty]
        private bool _isLoadingData;

        public ReportStore(IApiService apiService, ILogger<ReportStore> logger)
        {
            _apiService = apiService;
            _logger = logger;

            AvailableReportTypes = new(Enum.GetValues(typeof(ReportType)).Cast<ReportType>());
            AvailableAccounts = new();
            AvailableCategories = new();
            SortingCriteria = new();
            AvailableDocumentFormats = new(Enum.GetValues(typeof(DocumentFormat)).Cast<DocumentFormat>());
        }

        public async Task LoadInitialDataAsync()
        {
            if (IsLoadingData) return;

            if (AvailableAccounts.Any() || AvailableCategories.Any()) return;

            IsLoadingData = true;
            try
            {
                _logger.LogInformation("Loading initial data for ReportStore...");

                var accountsTask = _apiService.GetAsync<List<AccountDto>>("Account");
                var categoriesTask = _apiService.GetAsync<List<CategoryDto>>("categories");

                await Task.WhenAll(accountsTask, categoriesTask);

                var accountsFromApi = await accountsTask;
                var categoriesFromApi = await categoriesTask;

                AvailableAccounts.Clear();
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

                _logger.LogInformation("Initial data for ReportStore loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load initial data for ReportStore.");
                throw;
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        public async Task<string?> CreateAndSaveReportAsync(ReportRequestDto request)
        {
            _logger.LogInformation("Sending report creation request. Type: {ReportType}", request.ReportType);

            try
            {
                var result = await _apiService.PostAndDownloadReportAsync("Reports/generate", request);

                if (result.HasValue && result.Value.FileBytes.Length > 0)
                {
                    var (fileBytes, fileName) = result.Value;
                    string savedPath = await FileSaver.SaveReportToDocumentsAsync(fileBytes, fileName);
                    _logger.LogInformation("Report saved successfully: {Path}", savedPath);

                    return savedPath;
                }
                else
                {
                    _logger.LogWarning("No file data received from API or no data found for the report.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating and saving the report.");
                return null;
            }
        }
    }
}