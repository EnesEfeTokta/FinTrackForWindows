using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Services.Debts;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class DebtViewModel : ObservableObject
    {
        private readonly ILogger<DebtViewModel> _logger;
        private readonly IDebtStore _debtStore;

        public IDebtStore DebtStore => _debtStore;

        [ObservableProperty]
        private string? newProposalBorrowerEmail;

        [ObservableProperty]
        private decimal newProposalAmount;

        [ObservableProperty]
        private string newProposalDescription;

        [ObservableProperty]
        private DateTime newProposalDueDate = DateTime.Now.AddMonths(1);

        public DebtViewModel(ILogger<DebtViewModel> logger, IDebtStore debtStore)
        {
            _logger = logger;
            _debtStore = debtStore;
        }

        [RelayCommand]
        private async Task SendOfferAsync()
        {
            if (string.IsNullOrWhiteSpace(NewProposalBorrowerEmail) || NewProposalAmount <= 0)
            {
                MessageBox.Show("Lütfen borçlu e-postası ve geçerli bir miktar girin.", "Doğrulama Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _debtStore.SendOfferAsync(NewProposalBorrowerEmail, NewProposalAmount, "TRY", NewProposalDueDate, NewProposalDescription);

                NewProposalBorrowerEmail = string.Empty;
                NewProposalAmount = 0;
                NewProposalDescription = string.Empty;
                NewProposalDueDate = DateTime.Now.AddMonths(1);

                MessageBox.Show("Teklif başarıyla gönderildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send debt offer from ViewModel.");
                MessageBox.Show("Teklif gönderilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task RespondToOfferAsync(object parameter)
        {
            if (parameter is not object[] values || values.Length != 2 || values[0] is not DebtModel debt || values[1] is not bool decision) return;

            try
            {
                await _debtStore.RespondToOfferAsync(debt, decision);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to respond to offer from ViewModel for DebtId: {DebtId}", debt.Id);
                MessageBox.Show("Teklife yanıt verilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task UploadVideoAsync(DebtModel debt)
        {
            if (debt == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files (*.mp4;*.mov;*.wmv)|*.mp4;*.mov;*.wmv",
                Title = "Select an Approval Video"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _debtStore.UploadVideoAsync(debt, openFileDialog.FileName);
                    MessageBox.Show("Video başarıyla yüklendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload video from ViewModel for DebtId: {DebtId}", debt.Id);
                    MessageBox.Show("Video yüklenirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}