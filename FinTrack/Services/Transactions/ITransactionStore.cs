using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Models.Transaction;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Transactions
{
    public interface ITransactionStore
    {
        ReadOnlyObservableCollection<TransactionModel> Transactions { get; }

        event NotifyCollectionChangedEventHandler? TransactionsChanged;

        Task LoadTransactionsAsync();

        Task AddTransactionAsync(TransactionCreateDto newTransaction);
        Task UpdateTransactionAsync(int transactionId, TransactionCreateDto updatedTransaction);
        Task DeleteTransactionAsync(int transactionId);
    }
}
