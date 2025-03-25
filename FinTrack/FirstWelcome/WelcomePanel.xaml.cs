using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinTrack.FirstWelcome
{
    /// <summary>
    /// Interaction logic for WelcomePanel.xaml
    /// </summary>
    public partial class WelcomePanel : Page
    {
        public WelcomePanel()
        {
            InitializeComponent();

            Start();
        }

        private void Start()
        {
            DataContext = new FirstWelcomeSlideViewModel { SlideForwardButtonVisible = true };
            DataContext = new FirstWelcomeSlideViewModel { SlideBackButtonVisible = false };
            DataContext = new FirstWelcomeSlideViewModel { SlideSkipButtonVisible = true };
        }
    }
}
