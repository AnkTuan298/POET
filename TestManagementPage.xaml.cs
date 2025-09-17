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
using POET.Views;
using POET.Models;
using Microsoft.EntityFrameworkCore;

namespace POET
{
    public partial class TestManagementPage : UserControl
    {
        private Class _teacherClass;
        private readonly bool _isAdmin;

        public TestManagementPage()
        {
            InitializeComponent();
            _isAdmin = App.CurrentUser?.Role?.ToLower() == "admin";
            LoadTeacherClass();
            InitializeUI();
        }

        private void LoadTeacherClass()
        {
            if (_isAdmin) return; // Admins don't need a class

            using var context = new PoetContext();
            _teacherClass = context.Classes
                .Include(c => c.Tests)
                .FirstOrDefault(c => c.TeacherId == App.CurrentUser.UserId);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            if (mainWindow != null)
            {
                // Navigate back to test management
                mainWindow.contentFrame.Navigate(new TestManagementPage());

                // Reset navigation highlights
                mainWindow.ResetButtonStyles();
                mainWindow.subMenuPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                new CustomMessageBox("Navigation failed").ShowDialog();
            }
        }

        private void InitializeUI()
        {
            // Hide UI if teacher has no class
            if (!_isAdmin && _teacherClass == null)
            {
                txtNoClassWarning.Visibility = Visibility.Visible;
                testManagementPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoClassWarning.Visibility = Visibility.Collapsed;
                testManagementPanel.Visibility = Visibility.Visible;
                LoadTests();
            }
        }

        private void LoadTests()
        {
            using var context = new PoetContext();
            IQueryable<Test> query = context.Tests
                .Include(t => t.Level)
                .Include(t => t.Class)
                .Include(t => t.Questions);

            // Filter by class if teacher
            if (!_isAdmin)
                query = query.Where(t => t.ClassId == _teacherClass.ClassId);

            testsGrid.ItemsSource = query.ToList();
        }

        private void CreateTest_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TestEditorDialog(_isAdmin);
            if (dialog.ShowDialog() == true)
            {
                using var context = new PoetContext();
                var newTest = new Test
                {
                    TestName = dialog.TestName,
                    LevelId = dialog.SelectedLevelId,
                    ClassId = _isAdmin ? dialog.SelectedClassId : _teacherClass.ClassId,
                    CreatedBy = App.CurrentUser.UserId
                };

                context.Tests.Add(newTest);
                context.SaveChanges();
                LoadTests();
            }
        }

        private void DeleteTest_Click(object sender, RoutedEventArgs e)
        {
            var testId = (int)((Button)sender).Tag;
            bool confirm = YesNoMessageBox.Show("Delete this test?", "Confirm");

            if (confirm)
            {
                using var context = new PoetContext();
                var test = context.Tests
                    .Include(t => t.Questions)
                    .First(t => t.TestId == testId);

                context.Questions.RemoveRange(test.Questions);
                context.Tests.Remove(test);
                context.SaveChanges();
                LoadTests();
            }
        }

        private void EditQuestions_Click(object sender, RoutedEventArgs e)
        {
            var testId = (int)((Button)sender).Tag;

            try
            {
                using var context = new PoetContext();
                var testExists = context.Tests.Any(t => t.TestId == testId);

                if (!testExists)
                {
                    new CustomMessageBox("Test not found").ShowDialog();
                    return;
                }

                var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
                if (mainWindow != null)
                {
                    mainWindow.contentFrame.Navigate(new TestQuestionEditorPage(testId));
                }
                else
                {
                    new CustomMessageBox("Navigation failed").ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new ScrollableMessageBox($"Error: {ex.Message}\n\n{ex.StackTrace}").ShowDialog();
            }
        }
    }
}