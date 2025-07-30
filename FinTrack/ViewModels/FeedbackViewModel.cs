using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.FeedbackDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class FeedbackViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? inputSubject;

        [ObservableProperty]
        private FeedbackType selectedFeedbackType;

        [ObservableProperty]
        private string? inputDescription;

        [ObservableProperty]
        private string? selectedFilePath;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendFeedbackCommand))]
        private bool isSending = false;

        public IEnumerable<FeedbackType> FeedbackTypes { get; }

        private readonly ILogger<FeedbackViewModel> _logger;

        private readonly IApiService _apiService;

        public FeedbackViewModel(ILogger<FeedbackViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            FeedbackTypes = Enum.GetValues(typeof(FeedbackType)).Cast<FeedbackType>();
            SelectedFeedbackType = FeedbackTypes.FirstOrDefault();
        }

        [RelayCommand(CanExecute = nameof(CanSendFeedback))]
        private async Task SendFeedback()
        {
            if (IsSending) return;

            var newFeedback = new FeedbackCreateDto
            {
                Subject = InputSubject ?? "The title has been left blank.",
                Description = InputDescription ?? "The description has been left blank.",
                SavedFilePath = SelectedFilePath ?? "No file selected.",
                Type = SelectedFeedbackType,
            };

            await _apiService.PostAsync<object>("Feedback", newFeedback);

            // TODO: Burada sisteme bir e-posta göndermekte fayda var...

            _logger.LogInformation("Feedback submitted: Subject: {Subject}, Type: {Type}, Description: {Description}, File Path: {FilePath}",
                newFeedback.Subject, newFeedback.Type, newFeedback.Description, newFeedback.SavedFilePath);

            ClearForm();
        }

        private bool CanSendFeedback()
        {
            return !string.IsNullOrWhiteSpace(InputSubject) &&
                   !string.IsNullOrWhiteSpace(InputDescription) &&
                   !IsSending;
        }

        [RelayCommand]
        private void BrowseFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a file to attach",
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                _logger.LogInformation("Seçilen dosya: {FilePath}", SelectedFilePath);
            }
        }

        [RelayCommand]
        private void OpenLink(string? url)
        {
            if (string.IsNullOrEmpty(url) || url == "#") return;

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Link açılamadı: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError(ex, "Link açma hatası: {Url}", url);
            }
        }

        partial void OnInputSubjectChanged(string? value) => SendFeedbackCommand.NotifyCanExecuteChanged();
        partial void OnInputDescriptionChanged(string? value) => SendFeedbackCommand.NotifyCanExecuteChanged();

        private void ClearForm()
        {
            InputSubject = string.Empty;
            InputDescription = string.Empty;
            SelectedFilePath = null;
            SelectedFeedbackType = FeedbackTypes.FirstOrDefault();
        }
    }
}
