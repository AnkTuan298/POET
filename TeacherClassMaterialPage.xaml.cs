using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using POET.Models;
using POET.Views;

namespace POET
{
    public partial class TeacherClassMaterialPage : UserControl
    {
        private readonly PoetContext _context = new PoetContext();

        public TeacherClassMaterialPage()
        {
            InitializeComponent();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            var classId = _context.Classes
                .FirstOrDefault(c => c.TeacherId == App.CurrentUser.UserId)?.ClassId;

            if (classId != null)
            {
                var materials = _context.ClassMaterials
                    .Where(m => m.ClassId == classId)
                    .ToList();

                materialGrid.ItemsSource = materials;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddMaterialWindow();
            if (dialog.ShowDialog() == true)
            {
                var classId = _context.Classes
                    .First(c => c.TeacherId == App.CurrentUser.UserId).ClassId;

                var newMaterial = new ClassMaterial
                {
                    Name = dialog.MaterialName,
                    Url = dialog.MaterialUrl,
                    ClassId = classId
                };

                _context.ClassMaterials.Add(newMaterial);
                _context.SaveChanges();
                LoadMaterials();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (materialGrid.SelectedItem is ClassMaterial selected)
            {
                var dialog = new AddMaterialWindow(selected.Name, selected.Url);
                if (dialog.ShowDialog() == true)
                {
                    selected.Name = dialog.MaterialName;
                    selected.Url = dialog.MaterialUrl;

                    _context.SaveChanges();
                    LoadMaterials();
                }
            }
            else
            {
                MessageBox.Show("Please select a material to edit.");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (materialGrid.SelectedItem is ClassMaterial selected)
            {
                bool confirm = YesNoMessageBox.Show("Are you sure you want to delete this material?", "Confirm Delete");
                if (confirm)
                {
                    _context.ClassMaterials.Remove(selected);
                    _context.SaveChanges();
                    LoadMaterials();
                }
            }
            else
            {
                MessageBox.Show("Please select a material to delete.");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            if (mainWindow != null)
            {
                mainWindow.contentFrame.Navigate(new StudentListPage()); // hoặc trang nào cậu muốn
            }
        }

    }
}

