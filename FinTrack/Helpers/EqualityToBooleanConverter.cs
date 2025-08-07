using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FinTrackForWindows.Helpers
{
    public class EqualityToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v == null || v == DependencyProperty.UnsetValue))
            {
                return false;
            }

            for (int i = 1; i < values.Length; i++)
            {
                if (!object.Equals(values[0], values[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}