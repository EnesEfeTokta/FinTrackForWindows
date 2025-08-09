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

        public SelectableOptionReport(int id, string name, bool isSelected = false)
        {
            this.Id = id;
            this.name = name;
            this.isSelected = isSelected;
        }
    }
}