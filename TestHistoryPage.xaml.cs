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
    public partial class TestHistoryPage : UserControl
    {
        private readonly int? _studentId;

        public TestHistoryPage(int? studentId = null)
        {
            InitializeComponent();
            _studentId = studentId;
            LoadTestHistory();
        }

        private void LoadTestHistory()
        {
            using var context = new PoetContext();
            var query = context.TestHistories.AsQueryable();

            if (_studentId != null)
            {
                // Show history for specific student
                query = query.Where(th => th.UserId == _studentId);
            }
            else if (App.CurrentUser != null)
            {
                // Default: Show current user's history
                query = query.Where(th => th.UserId == App.CurrentUser.UserId);
            }

            historyGrid.ItemsSource = query
                .OrderByDescending(th => th.DateTaken)
                .ToList();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Determine role and navigate accordingly
            if (App.CurrentUser?.Role?.ToLower() == "teacher")
            {
                var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
                if (mainWindow != null)
                {
                    mainWindow.contentFrame.NavigationService.RemoveBackEntry();
                    mainWindow.contentFrame.Navigate(new StudentListPage());
                }
            }
            else
            {
                var mainWindow = Window.GetWindow(this) as UserMainWindow;
                if (mainWindow != null)
                {
                    mainWindow.contentFrame.NavigationService.RemoveBackEntry();
                    mainWindow.contentFrame.Navigate(new StudentProfilePage());
                }
            }
        }
    }
}
