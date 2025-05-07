using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FinTrack.FirstWelcome
{
    public class FirstWelcomeSlideViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FirstWelcomeSlideModel> slides = new ObservableCollection<FirstWelcomeSlideModel>();
        private int activeSlideIndex;

        public ObservableCollection<FirstWelcomeSlideModel> Slides
        {
            get => slides;
            set 
            { 
                slides = value; 
                OnPropertyChanged(); 
            }
        }

        public int ActiveSlideIndex
        {
            get => activeSlideIndex;
            set 
            { 
                activeSlideIndex = value; 
                OnPropertyChanged(); 
                UpdateUI(); 
            }
        }

        public FirstWelcomeSlideModel ActiveSlide => activeSlideIndex >= 0 && activeSlideIndex < Slides.Count ? Slides[activeSlideIndex] : null;

        public bool CanGoForward => activeSlideIndex < slides.Count - 1;
        public bool CanGoBack => activeSlideIndex > 0;
        public bool CanSkip => activeSlideIndex < slides.Count - 1;

        public FirstWelcomeSlideViewModel()
        {
            LoadSlidesFromJson();
            ActiveSlideIndex = 0;
        }

        public void GoForward()
        {
            if (CanGoForward)
            {
                ActiveSlideIndex++;
            }
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                ActiveSlideIndex--;
            }
        }

        public void Skip()
        {
            if (CanSkip)
            {
                ActiveSlideIndex = slides.Count - 1;
            }
        }

        private void UpdateUI()
        {
            // UI’yı güncellemek için gerekliyse burada ek mantık olabilir
        }

        private void LoadSlidesFromJson()
        {
            // JSON dosyasını oku
            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "FirstWelcome", "DB", "SlideDocument.json");
            string jsonContent = File.ReadAllText(jsonPath);

            // JSON’u deserialize et
            var slides = JsonConvert.DeserializeObject<ObservableCollection<FirstWelcomeSlideModel>>(jsonContent);
            if (slides != null)
            {
                Slides = slides;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}