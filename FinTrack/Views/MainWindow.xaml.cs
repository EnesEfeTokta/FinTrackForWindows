﻿using FinTrackForWindows.ViewModels;
using System.Windows;

namespace FinTrackForWindows.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            DataContext = _mainViewModel;

            this.Closing += (s, e) =>
            {
                Application.Current.Shutdown();
            };
        }
    }
}
