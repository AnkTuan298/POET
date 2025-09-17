using POET.Models;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using POET.Views;
using System.Text.RegularExpressions;

namespace POET
{
    public partial class StudentProfilePage : UserControl
    {
        private readonly PoetContext _context = new PoetContext();
        private Profile _currentProfile;

        // Parameterless constructor
        public StudentProfilePage()
        {
            InitializeComponent();

            if (App.CurrentUser == null)
            {
                MessageBox.Show("Session expired. Please login again.");
                return;
            }

            LoadProfile();
            DataContext = this;
        }

        private void LoadProfile()
        {
            _currentProfile = _context.Profiles
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == App.CurrentUser.UserId);

            if (_currentProfile == null)
            {
                _currentProfile = new Profile
                {
                    UserId = App.CurrentUser.UserId,
                    Name = "New User"
                };
                _context.Profiles.Add(_currentProfile);
                _context.SaveChanges();
            }

            Username = App.CurrentUser.Username;
            Profile = _currentProfile;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            if (mainWindow != null)
            {
                // Clear navigation history
                mainWindow.contentFrame.NavigationService.RemoveBackEntry();

                // Navigate to fresh WelcomePage
                mainWindow.contentFrame.Navigate(new WelcomePage());

                mainWindow.ResetNavigationButtons();

                if (mainWindow.contentFrame.Content is WelcomePage welcomePage)
                {
                    welcomePage.UpdateUsername(App.CurrentUser.Username);
                }
            }
        }

        public string Username { get; set; }
        public Profile Profile { get; set; }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Validation: Full Name (no digits)
            if (string.IsNullOrWhiteSpace(Profile.Name) || Profile.Name.Any(char.IsDigit))
            {
                // Dùng YesNoMessageBox làm thông báo lỗi (chỉ cần nhấn Yes để đóng)
                YesNoMessageBox.Show(
                    "Full Name is required and cannot contain numbers.",
                    "Validation Error"
                );
                return;
            }

            // Validation: Email (must contain '@')
            if (string.IsNullOrWhiteSpace(Profile.Email) || !Profile.Email.Contains("@"))
            {
                YesNoMessageBox.Show(
                    "Please enter a valid email address containing '@'.",
                    "Validation Error"
                );
                return;
            }

            // Validation: PhoneNumber (numeric, may start with '+')
            var phonePattern = new Regex(@"^\+?\d+$");
            if (string.IsNullOrWhiteSpace(Profile.PhoneNumber) || !phonePattern.IsMatch(Profile.PhoneNumber))
            {
                YesNoMessageBox.Show(
                    "Phone number invalid",
                    "Validation Error"
                );
                return;
            }

            // Lưu thay đổi và thông báo thành công
            _context.SaveChanges();
            new CustomMessageBox("Profile updated!").ShowDialog();
        }



        private void BtnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            mainWindow?.contentFrame.Navigate(new TestHistoryPage());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
            DataContext = this;
        }
    }
}