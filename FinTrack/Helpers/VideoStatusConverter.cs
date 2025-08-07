using System.Globalization;
using System.Windows.Data;

namespace FinTrackForWindows.Helpers
{
    public class VideoStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int?)
            {
                return (int?)value != null ? "Uploaded" : "Not Uploaded";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
