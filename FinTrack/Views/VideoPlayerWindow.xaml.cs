using System.Windows;
using System.IO;

namespace FinTrackForWindows.Views
{
    public partial class VideoPlayerWindow : Window
    {
        private readonly string _tempVideoPath;

        public VideoPlayerWindow(string tempVideoPath)
        {
            InitializeComponent();
            _tempVideoPath = tempVideoPath;

            mediaPlayer.Source = new Uri(_tempVideoPath);

            mediaPlayer.Play();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Close();

            if (File.Exists(_tempVideoPath))
            {
                try
                {
                    File.Delete(_tempVideoPath);
                    Console.WriteLine($"Temporary video file deleted successfully: {_tempVideoPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not delete temporary video file: {_tempVideoPath}. Error: {ex.Message}");
                }
            }
        }
    }
}
