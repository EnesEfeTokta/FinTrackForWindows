using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FinTrackForWindows.Services.Camera
{
    public class CameraService : ICameraService
    {
        private VideoCapture? _capture;
        private VideoWriter? _writer;
        private DispatcherTimer? _timer;

        private readonly ILogger<CameraService> _logger;

        public Action<BitmapSource>? OnFrameReady { get; set; }

        public CameraService(ILogger<CameraService> logger)
        {
            _logger = logger;
        }

        public bool InitializeCamera(int cameraIndex = 0)
        {
            try
            {
                _capture = new VideoCapture(cameraIndex);
                if (!_capture.IsOpened) return false;

                double fps = _capture.Get(CapProp.Fps);
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000 / (fps > 0 ? fps : 30))
                };
                _timer.Tick += Timer_Tick;
                _timer.Start();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize camera.");
                return false;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_capture == null || !_capture.IsOpened) return;

            using (Mat frame = _capture.QueryFrame())
            {
                if (frame != null)
                {
                    OnFrameReady?.Invoke(ConvertMatToBitmapSource(frame));

                    if (_writer != null && _writer.IsOpened)
                    {
                        _writer.Write(frame);
                    }
                }
            }
        }

        public string StartRecording()
        {
            if (_capture == null || !_capture.IsOpened)
                throw new InvalidOperationException("Camera is not initialized.");

            string tempPath = Path.GetTempPath();
            string outputFilePath = Path.Combine(tempPath, $"debt_video_{Guid.NewGuid()}.mp4");

            int frameWidth = (int)_capture.Get(CapProp.FrameWidth);
            int frameHeight = (int)_capture.Get(CapProp.FrameHeight);
            double fps = _capture.Get(CapProp.Fps);

            _writer = new VideoWriter(outputFilePath, VideoWriter.Fourcc('X', '2', '6', '4'), fps > 0 ? fps : 30, new System.Drawing.Size(frameWidth, frameHeight), true);
            return outputFilePath;
        }

        public void StopRecording()
        {
            _writer?.Dispose();
            _writer = null;
        }

        public void Release()
        {
            _timer?.Stop();
            _capture?.Dispose();
            _writer?.Dispose();
        }

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        private static BitmapSource ConvertMatToBitmapSource(Mat image)
        {
            using var bitmap = image.ToBitmap();
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
    }
}
