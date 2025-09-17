using POET.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace POET
{
    public partial class TestEditorDialog : Window
    {
        public string TestName { get; private set; }
        public int SelectedLevelId { get; private set; }
        public int? SelectedClassId { get; private set; }

        public TestEditorDialog(bool isAdmin)
        {
            InitializeComponent();
            InitializeForAdmin(isAdmin);
            LoadLevels();
        }

        private void InitializeForAdmin(bool isAdmin)
        {
            if (isAdmin)
            {
                cmbClasses.Visibility = Visibility.Visible;
                txtClassLabel.Visibility = Visibility.Visible;
                LoadClasses();
            }
        }

        private void LoadClasses()
        {
            using var context = new PoetContext();
            cmbClasses.ItemsSource = context.Classes.ToList();
            cmbClasses.SelectedIndex = 0;
        }

        private void LoadLevels()
        {
            using var context = new PoetContext();
            cmbLevels.ItemsSource = context.Levels.ToList();
            cmbLevels.SelectedIndex = 0;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTestName.Text))
            {
                MessageBox.Show("Test name is required!");
                return;
            }

            TestName = txtTestName.Text.Trim();
            SelectedLevelId = ((Level)cmbLevels.SelectedItem).LevelId;

            if (cmbClasses.SelectedItem != null)
                SelectedClassId = ((Class)cmbClasses.SelectedItem).ClassId;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}