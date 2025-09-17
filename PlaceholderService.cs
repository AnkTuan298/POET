using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POET.Helpers
{
    public class PlaceholderService
    {
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(PlaceholderService), new PropertyMetadata("", OnPlaceholderChanged));

        public static void SetPlaceholderText(UIElement element, string value)
        {
            element.SetValue(PlaceholderTextProperty, value);
        }

        public static string GetPlaceholderText(UIElement element)
        {
            return (string)element.GetValue(PlaceholderTextProperty);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Text = e.NewValue.ToString();
                textBox.Foreground = Brushes.Gray;

                textBox.GotFocus += (sender, args) =>
                {
                    if (textBox.Text == e.NewValue.ToString())
                    {
                        textBox.Text = "";
                        textBox.Foreground = Brushes.Black;
                    }
                };

                textBox.LostFocus += (sender, args) =>
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Text = e.NewValue.ToString();
                        textBox.Foreground = Brushes.Gray;
                    }
                };
            }
        }
    }
}