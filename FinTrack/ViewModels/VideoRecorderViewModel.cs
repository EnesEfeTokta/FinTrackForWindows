using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.Camera;
using Microsoft.Extensions.Logging;
using System.Windows.Media.Imaging;

namespace FinTrackForWindows.ViewModels
{
    public partial class VideoRecorderViewModel : ObservableObject
    {
        private readonly ICameraService _cameraService;
        private readonly Action<bool, string?> _closeWindowAction;

        [ObservableProperty]
        private BitmapSource? _cameraFrame;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RecordButtonText))]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool _isRecording;

        [ObservableProperty]
        private string? _recordedVideoPath;

        public string RecordButtonText => IsRecording ? "Stop Recording" : "Start Recording";
        public string CommitmentText { get; }

        private readonly ILogger<DebtViewModel> _logger;
        private readonly IAppInNotificationService _appInNotificationService;

        public VideoRecorderViewModel(DebtModel debt, 
            ICameraService cameraService, 
            Action<bool, string?> closeWindowAction,
            ILogger<DebtViewModel> logger,
            IAppInNotificationService appInNotificationService)
        {
            _cameraService = cameraService;
            _closeWindowAction = closeWindowAction;

            _logger = logger;
            _appInNotificationService = appInNotificationService;

            CommitmentText = $"I, {debt.BorrowerName}, acknowledge that I have received a loan in the amount of {debt.Amount:N2} {debt.Currency} from {debt.LenderName} on {DateTime.UtcNow:dd.MM.yyyy}. " +
                $"I undertake to make the payment no later than {debt.DueDate:dd.MM.yyyy}. If I fail to repay the debt in the specified amount and on the specified date, " +
                $"I acknowledge that this video recording will be made available to the lender {debt.LenderName} and may be used as legal evidence.";

            _cameraService.OnFrameReady = (frame) => CameraFrame = frame;

            if (!_cameraService.InitializeCamera())
            {
                _logger.LogError("There was a problem while recording the video.");
                _appInNotificationService.ShowError("There was a problem while recording the video.");
                _closeWindowAction(false, null);
            }
        }

        [RelayCommand]
        private void ToggleRecording()
        {
            IsRecording = !IsRecording;
            if (IsRecording)
            {
                RecordedVideoPath = _cameraService.StartRecording();
            }
            else
            {
                _cameraService.StopRecording();
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanSave() => !IsRecording && !string.IsNullOrEmpty(RecordedVideoPath);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            _closeWindowAction(true, RecordedVideoPath);
        }

        private bool CanCancel() => !IsRecording;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            _closeWindowAction(false, RecordedVideoPath);
        }

        [RelayCommand]
        private void Cleanup()
        {
            _cameraService.Release();
        }
    }
}
