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
    public partial class TeacherProfilePage : UserControl
    {
        private readonly PoetContext _context = new PoetContext();
        private Profile _currentProfile;
        private Class _teacherClass;

        public TeacherProfilePage()
        {
            InitializeComponent();
            LoadProfile();
            DataContext = this;
        }

        private void LoadProfile()
        {
            if (App.CurrentUser == null) return;

            // Load profile with class information
            _currentProfile = _context.Profiles
                .Include(p => p.User)
                    .ThenInclude(u => u.Classes)
                .FirstOrDefault(p => p.UserId == App.CurrentUser.UserId);

            // Get teacher's class
            _teacherClass = _context.Classes
                .FirstOrDefault(c => c.TeacherId == App.CurrentUser.UserId);

            // Set class name display
            txtClassName.Text = _teacherClass?.ClassName ?? "No class assigned";
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                new CustomMessageBox("Profile updated successfully!").ShowDialog();
            }
            catch (Exception ex)
            {
                new ScrollableMessageBox($"Error saving changes: {ex.Message}").ShowDialog();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            mainWindow?.contentFrame.Navigate(new WelcomePage());
            mainWindow?.ResetButtonStyles();
        }

        public string Username { get; set; }
        public Profile Profile => _currentProfile;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Username = App.CurrentUser?.Username ?? "Unknown";
            DataContext = null;
            DataContext = this;
        }
    }
}