using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinTrackForWindows.Helpers
{
    public class TextBoxMaskBehavior : Behavior<TextBox>
    {
        public static readonly System.Windows.DependencyProperty MaskProperty =
            System.Windows.DependencyProperty.Register("Mask", typeof(string), typeof(TextBoxMaskBehavior), new System.Windows.PropertyMetadata(null));

        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            if (string.IsNullOrEmpty(Mask)) return true;

            switch (Mask.ToLower())
            {
                case "integer":
                    return Regex.IsMatch(text, "[0-9]");
                case "decimal":
                    // Sadece sayı ve tek bir ondalık ayırıcıya izin ver
                    var currentText = AssociatedObject.Text;
                    var futureText = currentText.Insert(AssociatedObject.CaretIndex, text);
                    return Regex.IsMatch(futureText, @"^\d*\.?\d*$");
                default:
                    return Regex.IsMatch(text, Mask);
            }
        }
    }
}