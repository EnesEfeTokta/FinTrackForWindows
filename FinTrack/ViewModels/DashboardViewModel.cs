using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace FinTrack.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetDashboard> _budgets_DashboardView_ItemsControl;

        private readonly ILogger<DashboardViewModel> _logger;

        public DashboardViewModel(ILogger<DashboardViewModel> logger)
        {
            _logger = logger;
            LoadData();
        }

        private void LoadData()
        {
            Budgets_DashboardView_ItemsControl = new ObservableCollection<BudgetDashboard>
            {
                new BudgetDashboard { Name = "Tasarruf", DueDate = "15.02.2025", Amount = "6.000$", RemainingTime = "15 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") },
                new BudgetDashboard { Name = "Harcama", DueDate = "20.02.2025", Amount = "2.000$", RemainingTime = "10 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusRedBrush") },
                new BudgetDashboard { Name = "Yatırım", DueDate = "10.03.2025", Amount = "10.000$", RemainingTime = "30 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") },
                new BudgetDashboard { Name = "Eğlence", DueDate = "05.04.2025", Amount = "8.000$", RemainingTime = "20 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") }
            };
        }
    }
}
