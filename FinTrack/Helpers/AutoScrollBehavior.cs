using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace FinTrackForWindows.Helpers
{
    public static class AutoScrollBehavior
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached(
                "AutoScroll",
                typeof(bool),
                typeof(AutoScrollBehavior),
                new PropertyMetadata(false, OnAutoScrollPropertyChanged));

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        private static void OnAutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScrollViewer scrollViewer)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                scrollViewer.ScrollToBottom();

                if (FindItemsControl(scrollViewer) is { } itemsControl &&
                    itemsControl.ItemsSource is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged += (s, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                        {
                            scrollViewer.ScrollToBottom();
                        }
                    };
                }
            }
        }

        private static ItemsControl? FindItemsControl(DependencyObject parent)
        {
            if (parent == null) return null;

            if (parent is ItemsControl itemsControl)
            {
                return itemsControl;
            }

            if (parent is ContentPresenter contentPresenter)
            {
                if (contentPresenter.Content is ItemsControl ic)
                {
                    return ic;
                }
            }

            if (parent is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is ItemsControl ic)
                    {
                        return ic;
                    }
                }
            }

            return null;
        }
    }
}
