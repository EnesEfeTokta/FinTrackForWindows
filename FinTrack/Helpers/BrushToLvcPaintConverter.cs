using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FinTrackForWindows.Helpers
{
    public class BrushToLvcPaintConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return new SolidColorPaint(new SKColor(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A));
            }
            return new SolidColorPaint(SKColors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
