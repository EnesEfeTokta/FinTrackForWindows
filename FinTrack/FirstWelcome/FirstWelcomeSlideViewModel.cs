using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FinTrack.FirstWelcome
{
    public class FirstWelcomeSlideViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FirstWelcomeSlideModel> _slides = new ObservableCollection<FirstWelcomeSlideModel>();
        private int activeSlideIndex;

        private bool isSlideForwardButtonVisible;
        private bool isSlideBackButtonVisible;
        private bool isSlideSkipButtonVisible;

        public ObservableCollection<FirstWelcomeSlideModel> Slides
        {
            get => _slides;
            set
            {
                _slides = value;
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
            }
        }

        #region Page Navigation
        public bool SlideForwardButtonVisible
        {
            get => isSlideForwardButtonVisible;
            set
            {
                if (isSlideForwardButtonVisible != value)
                {
                    isSlideForwardButtonVisible = value;
                    OnPropertyChanged(nameof(isSlideForwardButtonVisible));
                }
            }
        }

        public bool SlideBackButtonVisible
        {
            get => isSlideBackButtonVisible;
            set
            {
                if (isSlideBackButtonVisible != value)
                {
                    isSlideBackButtonVisible = value;
                    OnPropertyChanged(nameof(isSlideBackButtonVisible));
                }
            }
        }

        public bool SlideSkipButtonVisible
        {
            get => isSlideSkipButtonVisible;
            set
            {
                if (isSlideSkipButtonVisible != value)
                {
                    isSlideSkipButtonVisible = value;
                    OnPropertyChanged(nameof(isSlideSkipButtonVisible));
                }
            }
        }
        #endregion

        public FirstWelcomeSlideModel? ActiveSlide => ActiveSlideIndex >= 0 && ActiveSlideIndex <
            Slides.Count ? Slides[ActiveSlideIndex] : null;

        public void GoToNextSlide()
        {
            if (activeSlideIndex < _slides.Count - 1)
            {
                ActiveSlideIndex++;
            }
        }

        public void GoToPreviousSlide()
        {
            if (activeSlideIndex > 0)
            {
                ActiveSlideIndex--;
            }
        }

        public void SkipSlide(int index)
        {
            if (index >= 0 && index < _slides.Count)
                activeSlideIndex = index;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ReadJsonData()
        {
            var json = System.IO.File.ReadAllText("/FirstWelcome/DB/SlideDocument.json");
            Slides = JsonConvert.DeserializeObject<ObservableCollection<FirstWelcomeSlideModel>>(json) ?? new ObservableCollection<FirstWelcomeSlideModel>();
        }
    }
}
