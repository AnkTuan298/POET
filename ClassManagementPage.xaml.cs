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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using POET.Models;
using System.Linq;
using POET.Views;

namespace POET
{
    public partial class ClassManagementPage : UserControl
    {
        public ClassManagementPage()
        {
            InitializeComponent();
            LoadClasses();
        }

        private void LoadClasses()
        {
            using var context = new PoetContext();
            classesGrid.ItemsSource = context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Students)
                .ToList();
        }

        private void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            var classId = (int)((Button)sender).Tag;

            bool confirm = YesNoMessageBox.Show(
                "Delete this class and all associated tests/students?",
                "Confirm Delete"
            );

            if (confirm)
            {
                using var context = new PoetContext();
                var classToDelete = context.Classes
                    .Include(c => c.Tests)
                        .ThenInclude(t => t.Questions)
                    .Include(c => c.Students)
                    .First(c => c.ClassId == classId);

                // Delete questions
                foreach (var test in classToDelete.Tests)
                {
                    context.Questions.RemoveRange(test.Questions);
                }

                // Delete tests
                context.Tests.RemoveRange(classToDelete.Tests);

                // Delete class
                context.Classes.Remove(classToDelete);
                context.SaveChanges();
                LoadClasses();
            }
        }

        private void ViewParticipants_Click(object sender, RoutedEventArgs e)
        {
            var classId = (int)((Button)sender).Tag;
            var mainWindow = Window.GetWindow(this) as AdminMainWindow;
            mainWindow?.contentFrame.Navigate(new ParticipantListPage(classId));
        }
    }
}
