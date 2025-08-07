using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Models.Debt;
using FinTrackForWindows.Services.Camera;
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

        public VideoRecorderViewModel(DebtModel debt, ICameraService cameraService, Action<bool, string?> closeWindowAction)
        {
            _cameraService = cameraService;
            _closeWindowAction = closeWindowAction;

            CommitmentText = $"Ben, {debt.BorrowerName}, {debt.LenderName} kişisinden {DateTime.UtcNow:dd.MM.yyyy} tarihinde almış olduğum {debt.Amount:N2} TRY tutarındaki borcu, " +
                $"en geç {debt.DueDate:dd.MM.yyyy} tarihinde ödemeyi taahhüt ediyorum. Eğer borcu belirtilen zamanda ve miktarda geri ödemezsem, " +
                $"bu video kaydının borç veren {debt.LenderName} kişisinin erişimine açılacağını ve yasal delil olarak kullanılabileceğini kabul ediyorum.";

            _cameraService.OnFrameReady = (frame) => CameraFrame = frame;

            if (!_cameraService.InitializeCamera())
            {
                // TODO: Show a user-friendly error message.
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
            // Bu metot, pencere kapandığında çağrılır.
            _cameraService.Release();
        }
    }
}
