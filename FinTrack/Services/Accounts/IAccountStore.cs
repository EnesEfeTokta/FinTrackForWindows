using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Models.Account;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FinTrackForWindows.Services.Accounts
{
    public interface IAccountStore
    {
        ReadOnlyObservableCollection<AccountModel> Accounts { get; }

        event NotifyCollectionChangedEventHandler? AccountsChanged;

        Task LoadAccountsAsync();

        Task AddAccountAsync(AccountCreateDto newBudget);
        Task UpdateAccountAsync(int accountId, AccountCreateDto updatedBudget);
        Task DeleteAccountAsync(int accountId);
    }
}
