using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Services.Camera;
using FinTrackForWindows.Services.Debts;
using FinTrackForWindows.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class DebtViewModel : ObservableObject
    {
        private readonly ILogger<DebtViewModel> _logger;
        private readonly IDebtStore _debtStore;

        private readonly IServiceProvider _serviceProvider;

        public IDebtStore DebtStore => _debtStore;

        [ObservableProperty]
        private string? newProposalBorrowerEmail;

        [ObservableProperty]
        private decimal newProposalAmount;

        [ObservableProperty]
        private string? newProposalDescription;

        [ObservableProperty]
        private DateTime newProposalDueDate = DateTime.Now.AddMonths(1);

        public DebtViewModel(ILogger<DebtViewModel> logger, IDebtStore debtStore, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _debtStore = debtStore;
            _serviceProvider = serviceProvider;
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
        private async Task UploadVideoAsync(DebtModel? debt)
        {
            if (debt == null)
            {
                _logger.LogWarning("UploadVideoAsync was called with a null debt model.");
                return;
            }

            using var cameraService = _serviceProvider.GetRequiredService<ICameraService>();

            string? videoPathToProcess = null;

            Action<bool, string?> closeAction = (isSuccess, filePath) =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is VideoRecorderWindow)
                    {
                        window.DialogResult = isSuccess;
                        videoPathToProcess = filePath;
                        window.Close();
                        break;
                    }
                }
            };

            var videoRecorderViewModel = new VideoRecorderViewModel(debt, cameraService, closeAction);
            var videoRecorderWindow = new VideoRecorderWindow
            {
                DataContext = videoRecorderViewModel
            };

            bool? dialogResult = videoRecorderWindow.ShowDialog();

            if (dialogResult == true && !string.IsNullOrEmpty(videoPathToProcess))
            {
                try
                {
                    await _debtStore.UploadVideoAsync(debt, videoPathToProcess);
                    MessageBox.Show("Video başarıyla yüklendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload video from ViewModel for DebtId: {DebtId}", debt.Id);
                    MessageBox.Show("Video yüklenirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // Yükleme başarılı da olsa başarısız da olsa geçici dosyayı SİLİNECEK.
                    try
                    {
                        File.Delete(videoPathToProcess);
                        _logger.LogInformation("Temporary video file deleted: {FilePath}", videoPathToProcess);
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogWarning(fileEx, "Could not delete temporary video file: {FilePath}", videoPathToProcess);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(videoPathToProcess))
            {
                // Kullanıcı "Cancel" ettiyse kaydedilmiş ama gönderilmemiş geçici dosyayı sil.
                try
                {
                    File.Delete(videoPathToProcess);
                    _logger.LogInformation("Video recording was canceled. Temporary file deleted: {FilePath}", videoPathToProcess);
                }
                catch (Exception fileEx)
                {
                    _logger.LogWarning(fileEx, "Could not delete temporary video file after cancellation: {FilePath}", videoPathToProcess);
                }
            }
        }

        [RelayCommand]
        private async Task MarkAsDefaultedAsync(DebtModel? debt)
        {
            if (debt == null) return;

            var result = MessageBox.Show(
                "Are you sure you want to mark this debt as defaulted? This action cannot be undone.",
                "Confirm Action",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _debtStore.MarkDebtAsDefaultedAsync(debt.Id);
                    MessageBox.Show("The debt has been successfully marked as defaulted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to execute MarkAsDefaulted command for DebtId: {DebtId}", debt.Id);
                    MessageBox.Show("An error occurred while marking the debt as defaulted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task ViewVideoAsync(DebtModel? debt)
        {
            if (debt?.VideoMetadataId == null) return;

            var keyEntryWindow = new KeyEntryWindow();
            if (keyEntryWindow.ShowDialog() == true)
            {
                string key = keyEntryWindow.EnteredKey;

                try
                {
                    var (videoStream, contentType) = await _debtStore.GetVideoStreamAsync(debt.VideoMetadataId.Value, key);

                    if (videoStream != null)
                    {
                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");
                        await using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await videoStream.CopyToAsync(fileStream);
                        }
                        videoStream.Close();

                        var videoPlayer = new VideoPlayerWindow(tempFilePath);
                        videoPlayer.Show();
                    }
                    else
                    {
                        MessageBox.Show("Could not retrieve the video. Please check the key and your permissions.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while trying to view the video for DebtId: {DebtId}", debt.Id);
                    MessageBox.Show("An error occurred while trying to view the video.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}