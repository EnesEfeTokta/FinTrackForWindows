using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FinTrack.FirstWelcome
{
    public partial class WelcomePanel : Page
    {
        private readonly FirstWelcomeSlideViewModel viewModel;

        public WelcomePanel()
        {
            InitializeComponent();
            viewModel = new FirstWelcomeSlideViewModel();
            UpdateUI();
        }

        private void SlideForwardButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GoForward();
            UpdateUI();
        }

        private void SlideBackButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GoBack();
            UpdateUI();
        }

        private void SlideSkipButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Skip();
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (viewModel.ActiveSlide != null)
            {
                BitmapImage newImage = new BitmapImage(new Uri($"pack://application:,,,/{viewModel.ActiveSlide.ImagePath}", UriKind.Absolute));
                newImage.CacheOption = BitmapCacheOption.OnLoad;
                SlideImage.Source = newImage;


                SlideHead.Text = viewModel.ActiveSlide.Header;
                SlideBody.Text = viewModel.ActiveSlide.Body;
            }
            SlideForwardButton.IsEnabled = viewModel.ActiveSlideIndex < viewModel.Slides.Count - 1;
        }
    }
}