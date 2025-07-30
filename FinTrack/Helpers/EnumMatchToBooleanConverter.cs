using System.Globalization;
using System.Windows.Data;

namespace FinTrackForWindows.Helpers
{
    public class EnumMatchToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            var value = values[0];
            var targetValue = values[1];

            if (value == null || targetValue == null)
                return false;

            return value.Equals(targetValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return new object[] { Binding.DoNothing, value };
            }
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}