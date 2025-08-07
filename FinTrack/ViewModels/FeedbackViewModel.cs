using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.FeedbackDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.Services.AppInNotifications;
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

        private readonly IAppInNotificationService _appInNotificationService;

        public FeedbackViewModel(ILogger<FeedbackViewModel> logger, IApiService apiService, IAppInNotificationService appInNotificationService)
        {
            _logger = logger;
            _apiService = apiService;
            _appInNotificationService = appInNotificationService;

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

            // TODO: It may be useful to send an email to the system here...

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
                _logger.LogInformation("Selected file: {FilePath}", SelectedFilePath);
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
                _appInNotificationService.ShowError("The link could not be opened.", ex);
                _logger.LogError(ex, "Error opening link: {Url}", url);
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
