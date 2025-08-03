using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Services.Api;
using System.Windows;

namespace FinTrackForWindows.Models.Transaction
{
    public partial class AddCategoryViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        public Action? CloseWindow { get; set; }
        public TransactionCategoryModel? CreatedCategory { get; private set; }

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private TransactionType type = TransactionType.Expense;

        public AddCategoryViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Kategori adı boş olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var createDto = new TransactionCategoryDto
            {
                Name = this.Name,
                Type = this.Type
            };

            var result = await _apiService.PostAsync<TransactionCategoryDto>("TransactionCategory", createDto);

            if (result != null)
            {
                CreatedCategory = new TransactionCategoryModel { Id = result.Id, Name = result.Name, Type = result.Type };
                CloseWindow?.Invoke();
            }
            else
            {
                MessageBox.Show("Kategori oluşturulurken bir hata oluştu.", "API Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
