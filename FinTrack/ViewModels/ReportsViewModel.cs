using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using FinTrack.Models.Report;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        public ObservableCollection<ReportType> AvailableReportTypes { get; }
        public ObservableCollection<SelectableOptionReport> AvailableAccounts { get; }
        public ObservableCollection<SelectableOptionReport> AvailableCategories { get; }
        public ObservableCollection<string> SortingCriteria { get; }
        public ObservableCollection<ExportFormat> AvailableExportFormats { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReportSummary))]
        private ReportType selectedReportType;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReportSummary))]
        private DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReportSummary))]
        private DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);

        [ObservableProperty]
        private bool isIncomeSelected = true;

        [ObservableProperty]
        private bool isExpenseSelected = true;

        [ObservableProperty]
        private string selectedSortingCriterion;

        [ObservableProperty]
        private ExportFormat selectedExportFormat;

        private readonly ILogger<ReportsViewModel> _logger;

        public string ReportSummary
        {
            get
            {
                var summary = new StringBuilder();
                if (SelectedReportType != null)
                {
                    summary.Append($"{SelectedReportType} Report, ");
                    summary.Append($"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy} ");
                    summary.Append("it will include all accounts and categories between the dates.");
                }
                return summary.ToString();
            }
        }

        public ReportsViewModel(ILogger<ReportsViewModel> logger)
        {
            _logger = logger;

            AvailableReportTypes = new ObservableCollection<ReportType>(Enum.GetValues(typeof(ReportType)).Cast<ReportType>());
            AvailableAccounts = new ObservableCollection<SelectableOptionReport>();
            AvailableCategories = new ObservableCollection<SelectableOptionReport>();
            SortingCriteria = new ObservableCollection<string>();
            AvailableExportFormats = new ObservableCollection<ExportFormat>(Enum.GetValues(typeof(ExportFormat)).Cast<ExportFormat>());

            LoadSampleData();
        }

        private void LoadSampleData()
        {
            AvailableAccounts.Clear();
            AvailableAccounts.Add(new SelectableOptionReport("All Accounts", true));
            AvailableAccounts.Add(new SelectableOptionReport("Cash"));
            AvailableAccounts.Add(new SelectableOptionReport("Bank Account - A"));
            AvailableAccounts.Add(new SelectableOptionReport("Credit Card - B"));

            AvailableCategories.Clear();
            AvailableCategories.Add(new SelectableOptionReport("All Categories", true));
            AvailableCategories.Add(new SelectableOptionReport("Groceries"));
            AvailableCategories.Add(new SelectableOptionReport("Salary"));
            AvailableCategories.Add(new SelectableOptionReport("Rent"));

            SortingCriteria.Clear();
            SortingCriteria.Add("By Date (Newest to Oldest)");
            SortingCriteria.Add("By Date (Oldest to Newest)");
            SortingCriteria.Add("By Amount (Highest to Lowest)");
            SortingCriteria.Add("By Amount (Lowest to Highest)");
            SelectedReportType = AvailableReportTypes.FirstOrDefault();
            SelectedSortingCriterion = SortingCriteria.FirstOrDefault();
            SelectedExportFormat = AvailableExportFormats.FirstOrDefault();
        }

        [RelayCommand]
        private void SelectExportFormat(ExportFormat format)
        {
            SelectedExportFormat = format;
            _logger?.LogInformation("Selected export format: {ExportFormat}", format);
        }

        [RelayCommand]
        private void CreateReport()
        {
            _logger?.LogInformation("Aşağıdaki parametrelerle rapor oluşturma: {ReportType}, {StartDate} ila {EndDate}, Seçilen Gelir: {IsIncomeSelected}, Seçilen Gider: {IsExpenseSelected}, Sıralama Kriterleri: {SortingCriteria}, Dışa Aktarma Biçimi: {ExportFormat}",
             SelectedReportType, StartDate, EndDate, IsIncomeSelected, IsExpenseSelected, SelectedSortingCriterion, SelectedExportFormat);

            MessageBox.Show("Report creation logic triggered! Check your logs.", "Success");
        }
    }
}