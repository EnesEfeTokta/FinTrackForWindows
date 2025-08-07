using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FinTrackForWindows.Helpers
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;
            if (value is bool b)
            {
                boolValue = b;
            }

            if (parameter != null && (parameter.ToString().Equals("invert", StringComparison.OrdinalIgnoreCase) ||
                                       parameter.ToString().Equals("true", StringComparison.OrdinalIgnoreCase)))
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility && visibility == Visibility.Visible)
            {
                if (parameter != null && (parameter.ToString().Equals("invert", StringComparison.OrdinalIgnoreCase) ||
                                           parameter.ToString().Equals("true", StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}