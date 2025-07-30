using System.Windows;
using System.Windows.Documents;

namespace FinTrackForWindows.Helpers
{
    public static class IsBusyAdornerBehavior
    {
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.RegisterAttached(
                "IsBusy",
                typeof(bool),
                typeof(IsBusyAdornerBehavior),
                new FrameworkPropertyMetadata(false, OnIsBusyChanged));

        public static bool GetIsBusy(DependencyObject d)
        {
            return (bool)d.GetValue(IsBusyProperty);
        }

        public static void SetIsBusy(DependencyObject d, bool value)
        {
            d.SetValue(IsBusyProperty, value);
        }

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null) return;

            if ((bool)e.NewValue)
            {
                var adorner = new BusyAdorner(element);
                adornerLayer.Add(adorner);
            }
            else
            {
                var adorners = adornerLayer.GetAdorners(element);
                if (adorners != null)
                {
                    foreach (var adorner in adorners)
                    {
                        if (adorner is BusyAdorner)
                        {
                            adornerLayer.Remove(adorner);
                        }
                    }
                }
            }
        }
    }
}