using FinTrackForWindows.Models.Debt;
using System.Collections.ObjectModel;
using System.IO;

namespace FinTrackForWindows.Services.Debts
{
    public interface IDebtStore
    {
        ReadOnlyObservableCollection<DebtModel> PendingOffers { get; }
        ReadOnlyObservableCollection<DebtModel> MyDebtsList { get; }
        ReadOnlyObservableCollection<DebtModel> ActiveDebts { get; }

        bool IsLoading { get; }

        Task LoadDebtsAsync();
        Task SendOfferAsync(string borrowerEmail, decimal amount, string currency, DateTime dueDate, string description);
        Task RespondToOfferAsync(DebtModel debt, bool accepted);
        Task UploadVideoAsync(DebtModel debt, string filePath);

        event Action DebtsChanged;

        Task MarkDebtAsDefaultedAsync(int debtId);

        Task<(Stream? Stream, string? ContentType)> GetVideoStreamAsync(int videoId, string key);
    }
}