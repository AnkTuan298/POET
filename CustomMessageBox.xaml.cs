using System.Windows;

namespace POET.Views
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close when OK is clicked
        }
    }
}
