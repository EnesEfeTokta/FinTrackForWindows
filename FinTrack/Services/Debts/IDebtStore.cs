using FinTrackForWindows.Models.Debt;
using System.Collections.ObjectModel;

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
    }
}