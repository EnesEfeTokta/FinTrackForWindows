using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTrackForWindows.Models.Report
{
    public partial class SelectableOptionReport : ObservableObject
    {
        public int Id { get; }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isSelected;

        public SelectableOptionReport(int _id, string _name, bool _isSelected = false)
        {
            name = _name;
            isSelected = _isSelected;
            Id = _id;
        }
    }
}
