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

namespace POET
{
    public partial class TestInfoPage : UserControl
    {
        private readonly Test _selectedTest;

        public TestInfoPage(Test selectedTest)
        {
            InitializeComponent();
            _selectedTest = selectedTest;
            DataContext = _selectedTest; // Bind the test data to the UI
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            // Get the main window to access navigation controls
            var mainWindow = Window.GetWindow(this) as UserMainWindow;

            if (mainWindow != null)
            {
                var currentUser = mainWindow.CurrentUser;
                // Hide navigation buttons
                mainWindow.btnProfile.Visibility = Visibility.Collapsed;
                mainWindow.btnTakeTest.Visibility = Visibility.Collapsed;
                mainWindow.btnJoinClass.Visibility = Visibility.Collapsed;
                mainWindow.txtPoetLogo.Visibility = Visibility.Collapsed;
                mainWindow.btnClass.Visibility = Visibility.Collapsed;
                mainWindow.btnRoleUpgrade.Visibility = Visibility.Collapsed;
                mainWindow.btnLogout.Visibility = Visibility.Collapsed;
                // Navigate to the test page with the selected test
                mainWindow.contentFrame.Navigate(new TestPage(_selectedTest, mainWindow.CurrentUser));
            }



        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            if (mainWindow != null)
            {
                // Clear navigation history
                mainWindow.contentFrame.NavigationService.RemoveBackEntry();

                // Navigate to fresh WelcomePage
                mainWindow.contentFrame.Navigate(new TakeTestPage());

            }
        }

    }
}