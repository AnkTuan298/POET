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
using POET.Models;
using POET.Views;

namespace POET
{
    public partial class TeacherMainWindow : Window
    {
        internal bool _isClassMenuOpen = false;

        public TeacherMainWindow()
        {
            InitializeComponent();

            // Verify teacher role on startup
            if (App.CurrentUser?.Role?.ToLower() != "teacher")
            {
                MessageBox.Show("Unauthorized access");
                this.Close();
                return;
            }

            InitializeNavigation();
            ShowWelcomePage();

        }

        private void InitializeNavigation()
        {
            contentFrame.Navigating += (sender, e) =>
            {
                // Optional: Add fade animations here
            };
        }

        private void ShowWelcomePage()
        {
            var welcomePage = new WelcomePage();
            welcomePage.txtUsername.Text = App.CurrentUser.Username;
            contentFrame.Navigate(welcomePage);
        }

        // Toggle Class submenu
        private void Class_Click(object sender, RoutedEventArgs e)
        {
            _isClassMenuOpen = !_isClassMenuOpen;
            subMenuPanel.Visibility = _isClassMenuOpen ? Visibility.Visible : Visibility.Collapsed;
            ResetButtonStyles();
        }

        // Handle Students/Tests navigation
        private void SubMenu_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ResetButtonStyles();
            button.Background = Brushes.LightGray;

            switch (button.Name)
            {
                case "btnStudents":
                    contentFrame.Navigate(new StudentListPage());
                    break;
                case "btnTests":
                    contentFrame.Navigate(new TestManagementPage());
                    break;
                case "btnCreateClass":
                    contentFrame.Navigate(new ClassCreationPage());
                    break;

            }
        }

        // Reset button highlights
        public void ResetButtonStyles()
        {
            btnProfile.ClearValue(BackgroundProperty);
            btnStudents.ClearValue(BackgroundProperty);
            btnTests.ClearValue(BackgroundProperty);
            btnCreateClass.ClearValue(BackgroundProperty);
        }

        // Logout
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            bool confirm = YesNoMessageBox.Show("Are you sure you want to log out?", "Logout");
            if (confirm)
            {
                App.CurrentUser = null;
                new LoginWindow().Show();
                this.Close();
            }
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonStyles();
            btnProfile.Background = Brushes.LightGray;

            // Navigate to ProfilePage
            contentFrame.Navigate(new TeacherProfilePage());
        }

        // Minimize/Close Window
        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        // Return to home on logo click
        private void PoetLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var welcomePage = new WelcomePage();
            welcomePage.UpdateUsername(App.CurrentUser.Username);
            contentFrame.Navigate(welcomePage);
            ResetButtonStyles();
            subMenuPanel.Visibility = Visibility.Collapsed;
        }
    }
}
