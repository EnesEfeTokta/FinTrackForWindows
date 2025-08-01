using FinTrackForWindows.Models;
using FinTrackForWindows.Models.Transaction;
using FinTrackForWindows.Services.Api;
using System.Windows;

namespace FinTrackForWindows.Views
{
    public partial class AddCategoryWindow : Window
    {
        private readonly AddCategoryViewModel _viewModel;

        public AddCategoryWindow(IApiService apiService)
        {
            InitializeComponent();
            _viewModel = new AddCategoryViewModel(apiService);
            DataContext = _viewModel;

            _viewModel.CloseWindow = () => {
                this.DialogResult = true;
                this.Close();
            };
        }

        public TransactionCategoryModel? GetCreatedCategory()
        {
            return _viewModel.CreatedCategory;
        }
    }
}
