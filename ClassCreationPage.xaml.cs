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
using POET.Models;
using POET.Views;
using POET;
using Microsoft.EntityFrameworkCore;

namespace POET
{
    public partial class ClassCreationPage : UserControl
    {
        private readonly int _teacherId;

        public ClassCreationPage()
        {
            InitializeComponent();
            _teacherId = App.CurrentUser.UserId;
            CheckExistingClass();
        }

        private void CheckExistingClass()
        {
            try
            {
                using var context = new PoetContext();
                var existingClass = context.Classes
                    .Include(c => c.Tests)
                    .FirstOrDefault(c => c.TeacherId == _teacherId);

                if (existingClass != null)
                {
                    txtExistingClass.Text = $"You already own: {existingClass.ClassName} (Code: {existingClass.ClassCode})";
                    txtExistingClass.Visibility = Visibility.Visible;
                    btnDeleteClass.Visibility = Visibility.Visible;
                    createClassPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtExistingClass.Visibility = Visibility.Collapsed;
                    btnDeleteClass.Visibility = Visibility.Collapsed;
                    createClassPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                new ScrollableMessageBox($"Error: {ex.Message}").ShowDialog();
            }
        }

        private void CreateClass_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClassName.Text))
            {
                new CustomMessageBox("Please enter a class name").ShowDialog();
                return;
            }

            try
            {
                using var context = new PoetContext();
                var newClass = new Class
                {
                    ClassName = txtClassName.Text.Trim(),
                    TeacherId = _teacherId,
                    ClassCode = GenerateClassCode()
                };

                context.Classes.Add(newClass);
                context.SaveChanges();

                new ScrollableMessageBox(
                    $"Class created successfully!\n\n" +
                    $"Class Name: {newClass.ClassName}\n" +
                    $"Class Code: {newClass.ClassCode}"
                ).ShowDialog();

                CheckExistingClass();
            }
            catch (Exception ex)
            {
                new ScrollableMessageBox($"Error creating class: {ex.Message}").ShowDialog();
            }
        }

        private void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            bool confirm = YesNoMessageBox.Show(
                "WARNING: Deleting this class will also remove all associated tests and student enrollments.\n\nContinue?",
                "Confirm Delete"
            );

            if (!confirm) return;

            try
            {
                using var context = new PoetContext();
                var existingClass = context.Classes
                    .Include(c => c.Tests)
                        .ThenInclude(t => t.Questions)
                    .Include(c => c.Students)
                    .FirstOrDefault(c => c.TeacherId == _teacherId);

                if (existingClass != null)
                {
                    // Delete all questions in tests
                    foreach (var test in existingClass.Tests)
                    {
                        context.Questions.RemoveRange(test.Questions);
                    }

                    // Delete all tests
                    context.Tests.RemoveRange(existingClass.Tests);

                    // Remove student enrollments
                    existingClass.Students.Clear();

                    // Delete the class
                    context.Classes.Remove(existingClass);
                    context.SaveChanges();

                    new CustomMessageBox("Class deleted successfully!").ShowDialog();
                    CheckExistingClass(); // Refresh UI
                }
            }
            catch (Exception ex)
            {
                new ScrollableMessageBox($"Delete failed: {ex.Message}").ShowDialog();
            }
        }

        private string GenerateClassCode()
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", "")
                .Substring(0, 6)
                .ToUpper();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            if (mainWindow != null)
            {
                // Collapse the class submenu
                mainWindow.subMenuPanel.Visibility = Visibility.Collapsed;
                mainWindow._isClassMenuOpen = false;

                // Navigate to welcome page and reset styles
                mainWindow.contentFrame.Navigate(new WelcomePage());
                mainWindow.ResetButtonStyles();
            }
        }
    }
}