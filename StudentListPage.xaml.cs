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
using POET.Views;

namespace POET
{
    public partial class StudentListPage : UserControl
    {
        private readonly PoetContext _context = new PoetContext();

        public StudentListPage()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void LoadStudents()
        {
            if (App.CurrentUser == null) return;

            // Get teacher's class and students
            var teacherClass = _context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.Profile)
                .FirstOrDefault(c => c.TeacherId == App.CurrentUser.UserId);

            if (teacherClass != null)
            {
                studentsGrid.ItemsSource = teacherClass.Students.ToList();
            }
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            var studentId = (int)((Button)sender).Tag;

            bool confirm = YesNoMessageBox.Show(
                "Remove this student from your class?",
                "Confirm Removal"
            );

            if (confirm)
            {
                var teacherClass = _context.Classes
                    .Include(c => c.Students)
                    .First(c => c.TeacherId == App.CurrentUser.UserId);

                var student = teacherClass.Students
                    .First(s => s.UserId == studentId);

                teacherClass.Students.Remove(student);
                _context.SaveChanges();
                LoadStudents(); // Refresh the list
            }
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            var studentId = (int)((Button)sender).Tag;
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            mainWindow?.contentFrame.Navigate(new TestHistoryPage(studentId));
        }

        private void BtnYourMaterial_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            if (mainWindow != null)
            {
                mainWindow.contentFrame.Navigate(new TeacherClassMaterialPage());
            }
        }

        private void BtnViewProgress_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            if (mainWindow != null)
            {
                mainWindow.contentFrame.Navigate(new ViewStudentProgressPage());
            }
        }
    }
}
