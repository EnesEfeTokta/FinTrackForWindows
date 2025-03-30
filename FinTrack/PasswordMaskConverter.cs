using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace FinTrack
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BindPassword =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxAssistant),
            new PropertyMetadata(false, OnBindPasswordChanged));

        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxAssistant),
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant),
            new PropertyMetadata(false));

        public static void SetBindPassword(DependencyObject dp, bool value) => dp.SetValue(BindPassword, value);
        public static bool GetBindPassword(DependencyObject dp) => (bool)dp.GetValue(BindPassword);

        public static void SetBoundPassword(DependencyObject dp, string value) => dp.SetValue(BoundPassword, value);
        public static string GetBoundPassword(DependencyObject dp) => (string)dp.GetValue(BoundPassword);

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is not PasswordBox passwordBox) return;

            if ((bool)e.OldValue)
                passwordBox.PasswordChanged -= PasswordChanged;

            if ((bool)e.NewValue)
                passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox && !GetUpdatingPassword(passwordBox))
                passwordBox.Password = e.NewValue?.ToString();
        }

        private static bool GetUpdatingPassword(DependencyObject dp) => (bool)dp.GetValue(UpdatingPassword);
        private static void SetUpdatingPassword(DependencyObject dp, bool value) => dp.SetValue(UpdatingPassword, value);
    }
}
