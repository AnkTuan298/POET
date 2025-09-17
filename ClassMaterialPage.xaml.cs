using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using POET.Models;
using POET.Views;

namespace POET
{
    public partial class ClassMaterialPage : UserControl
    {
        private readonly PoetContext _context = new PoetContext();

        public ClassMaterialPage()
        {
            InitializeComponent();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            var classId = _context.Classes
                .FirstOrDefault(c => c.Students.Any(s => s.UserId == App.CurrentUser.UserId))?.ClassId;

            if (classId != null)
            {
                var materials = _context.ClassMaterials
                    .Where(m => m.ClassId == classId)
                    .ToList();

                materialGrid.ItemsSource = materials;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            if (mainWindow != null)
            {
                mainWindow.contentFrame.Navigate(new YourClassPage());
            }
        }

        private void MaterialLink_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                var url = textBlock.Text;
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("Unable to open link.");
                }
            }
        }
    }
}

