using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FinTrack.Helpers
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Enum enumValue)
                return value;

            return GetEnumDescription(enumValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string description)
            {
                foreach (Enum enumValue in Enum.GetValues(targetType))
                {
                    if (GetEnumDescription(enumValue) == description)
                    {
                        return enumValue;
                    }
                }
            }

            return DependencyProperty.UnsetValue;
        }

        private string GetEnumDescription(Enum enumObj)
        {
            var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo == null) return enumObj.ToString();

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumObj.ToString();
        }
    }
}
