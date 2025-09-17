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
using System.Windows;

namespace POET
{
    public partial class AddMaterialWindow : Window
    {
        public string MaterialName { get; private set; }
        public string MaterialUrl { get; private set; }

        public AddMaterialWindow(string name = "", string url = "")
        {
            InitializeComponent();
            txtName.Text = name;
            txtUrl.Text = url;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MaterialName = txtName.Text.Trim();
            MaterialUrl = txtUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(MaterialName) || string.IsNullOrWhiteSpace(MaterialUrl))
            {
                MessageBox.Show("Please enter both a name and a URL.");
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
