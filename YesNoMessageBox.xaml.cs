using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POET.Views
{
    public partial class YesNoMessageBox : Window
    {
        // DependencyProperty for binding the message
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(YesNoMessageBox));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        // Constructor with parameters
        public YesNoMessageBox(string message, string title = "Confirmation")
        {
            InitializeComponent();
            DataContext = this; // Set DataContext to this window
            Message = message;  // Now binds to the XAML
            Title = title;
        }

        // Static method to show the dialog
        public static bool Show(string message, string title = "Confirmation")
        {
            var dialog = new YesNoMessageBox(message, title);
            return dialog.ShowDialog() == true;
        }

        // Button click handlers
        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
