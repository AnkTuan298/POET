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

namespace POET
{
    /// <summary>
    /// Interaction logic for ScrollableMessageBox.xaml
    /// </summary>
    public partial class ScrollableMessageBox : Window
    {
        public ScrollableMessageBox(string message, string title = "")
        {
            InitializeComponent();
            Title = title;
            txtMessage.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
