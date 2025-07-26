using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTrackForWindows.Models.Report
{
    public partial class SelectableOptionReport : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isSelected;

        public SelectableOptionReport(string _name, bool _isSelected = false)
        {
            name = _name;
            isSelected = _isSelected;
        }
    }
}
