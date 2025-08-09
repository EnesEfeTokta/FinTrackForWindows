using FinTrackForWindows.Dtos.ReportDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Report;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.Services.Reports
{
    public interface IReportStore
    {
        ObservableCollection<ReportType> AvailableReportTypes { get; }
        ObservableCollection<SelectableOptionReport> AvailableAccounts { get; }
        ObservableCollection<SelectableOptionReport> AvailableCategories { get; }
        ObservableCollection<string> SortingCriteria { get; }
        ObservableCollection<DocumentFormat> AvailableDocumentFormats { get; }

        bool IsLoadingData { get; }

        Task LoadInitialDataAsync();

        Task<string?> CreateAndSaveReportAsync(ReportRequestDto request);
    }
}
