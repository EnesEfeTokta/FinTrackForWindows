using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FinTrackForWindows.Helpers
{
    public class BusyAdorner : Adorner
    {
        private readonly StackPanel _child;

        public BusyAdorner(UIElement adornedElement) : base(adornedElement)
        {
            var progressBar = new ProgressBar
            {
                IsIndeterminate = true,
                Width = 100,
                Height = 20
            };

            var textBlock = new TextBlock
            {
                Text = "Creating Report...",
                Foreground = Brushes.White,
                Margin = new Thickness(0, 15, 0, 0),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            _child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            _child.Children.Add(progressBar);
            _child.Children.Add(textBlock);

            AddVisualChild(_child);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            AdornedElement.Measure(constraint);
            return AdornedElement.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)), null, new Rect(RenderSize));
            base.OnRender(drawingContext);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _child;
    }
}