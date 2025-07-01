using FinTrack.ViewModels;
using System.Windows;

namespace FinTrack.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            DataContext = _mainViewModel;
        }
    }
}
