using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;

namespace FinTrackForWindows.Models.Transaction
{
    public partial class TransactionCategoryModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private TransactionType type;
    }
}
