using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FinTrack
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        #region Navigation Properties
        private Uri currentPage;
        private Dictionary<string, Uri> panelDictionary = new Dictionary<string, Uri>
        {
            { "Dashboard", new Uri("Panels/DashboardPanel.xaml", UriKind.Relative) },
            { "Accounting", new Uri("Panels/AccountingPanel.xaml", UriKind.Relative) },
            { "Budgeting", new Uri("Panels/BudgetingPanel.xaml", UriKind.Relative) },
            { "Targets", new Uri("Panels/AccountPanel.xaml", UriKind.Relative) },
            { "Reports", new Uri("Panels/ReportsPanel.xaml", UriKind.Relative) },
            { "Account", new Uri("Panels/TargetsPanel.xaml", UriKind.Relative) }
        };

        public ICommand NavigateCommand { get; }

        private string panelHeadText { get; set; } = "Dashboard";
        #endregion

        public MainWindowViewModel()
        {
            CurrentPage = panelDictionary["Dashboard"];
            NavigateCommand = new RelayCommand(Navigate);
        }

        #region Navigation Methods
        public Uri CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        private void Navigate(object parameter)
        {
            if (parameter is string pageName)
            {
                CurrentPage = panelDictionary[pageName];
                PanelHeadText = pageName;
            }
        }

        public string PanelHeadText
        {
            get => panelHeadText;
            set
            {
                panelHeadText = value;
                OnPropertyChanged(nameof(PanelHeadText));
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
