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
    public partial class RoleUpgradeDialog : Window
    {
        public string SelectedRole { get; private set; }

        public RoleUpgradeDialog()
        {
            InitializeComponent();
            cmbRoles.SelectedIndex = 0;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRoles.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedRole = selectedItem.Content.ToString();
                DialogResult = true;
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
