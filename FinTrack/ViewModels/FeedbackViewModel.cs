using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class FeedbackViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? inputSubject;

        [ObservableProperty]
        private FeedbackTypes selectedFeedbackType;

        [ObservableProperty]
        private string? inputDescription;

        [ObservableProperty]
        private string? selectedFilePath;

        public IEnumerable<FeedbackTypes> FeedbackTypes { get; }

        private readonly ILogger<FeedbackViewModel> _logger;

        public FeedbackViewModel(ILogger<FeedbackViewModel> logger)
        {
            _logger = logger;

            FeedbackTypes = Enum.GetValues(typeof(FeedbackTypes)).Cast<FeedbackTypes>();
            SelectedFeedbackType = FeedbackTypes.FirstOrDefault();
        }

        [RelayCommand(CanExecute = nameof(CanSendFeedback))]
        private void SendFeedback()
        {
            string subject = InputSubject ?? "The title has been left blank.";
            string description = InputDescription ?? "The description has been left blank.";
            string filePath = SelectedFilePath ?? "No file selected.";
            string feedbackType = SelectedFeedbackType.ToString();

            string feedbackMessage = $"Subject: {subject}\n" +
                                      $"Type: {feedbackType}\n" +
                                      $"Description: {description}\n" +
                                      $"File Path: {filePath}";

            // TODO: feedbackMessage should be sent to a server or an email...

            MessageBox.Show(feedbackMessage, "Feedback Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
            _logger.LogInformation("Feedback submitted: Subject: {Subject}, Type: {Type}, Description: {Description}, File Path: {FilePath}",
                subject, feedbackType, description, filePath);

            ClearForm();
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

        private bool CanSendFeedback()
        {
            return !string.IsNullOrWhiteSpace(InputSubject) &&
                   !string.IsNullOrWhiteSpace(InputDescription);
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
