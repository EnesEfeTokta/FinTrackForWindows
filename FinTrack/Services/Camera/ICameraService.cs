using System.Windows.Media.Imaging;

namespace FinTrackForWindows.Services.Camera
{
    public interface ICameraService : IDisposable
    {
        Action<BitmapSource> OnFrameReady { get; set; }
        bool InitializeCamera(int cameraIndex = 0);
        string StartRecording();
        void StopRecording();
        void Release();
    }
}
