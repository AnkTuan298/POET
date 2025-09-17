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
using Microsoft.EntityFrameworkCore;

namespace POET
{
    public partial class TakeTestPage : UserControl
    {
        public TakeTestPage()
        {
            InitializeComponent();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;

            int testId = int.Parse(button.Tag.ToString());

            using (var context = new PoetContext())
            {
                // Include both Level and Questions
                var test = context.Tests
                    .Include(t => t.Level)          // Include Level data
                    .Include(t => t.Questions)       // Include Questions
                    .FirstOrDefault(t => t.TestId == testId);

                if (test != null)
                {
                    var mainWindow = Window.GetWindow(this) as UserMainWindow;
                    mainWindow?.contentFrame.Navigate(new TestInfoPage(test));
                }
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
                mainWindow.contentFrame.Navigate(new WelcomePage());

                mainWindow.ResetNavigationButtons();

                if (mainWindow.contentFrame.Content is WelcomePage welcomePage)
                {
                    welcomePage.UpdateUsername(App.CurrentUser.Username);
                }
            }
        }

        private void JoinClass_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            mainWindow?.contentFrame.Navigate(new JoinClassPage());
            mainWindow.ResetNavigationButtons();
        }

        private void RandomTest_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new PoetContext())
            {
                // Get all available test IDs
                var availableTestIds = context.Tests
                    .Where(t => t.ClassId == null) // Or other availability criteria
                    .Select(t => t.TestId)
                    .ToList();

                if (!availableTestIds.Any())
                {
                    MessageBox.Show("No available tests found!");
                    return;
                }

                // Select random test ID
                var random = new Random();
                int randomIndex = random.Next(availableTestIds.Count);
                int randomTestId = availableTestIds[randomIndex];

                // Get full test details with includes
                var randomTest = context.Tests
                    .Include(t => t.Level)
                    .Include(t => t.Questions)
                    .FirstOrDefault(t => t.TestId == randomTestId);

                if (randomTest != null)
                {
                    var mainWindow = Window.GetWindow(this) as UserMainWindow;
                    mainWindow?.contentFrame.Navigate(new TestInfoPage(randomTest));

                    // Optional: Create test history entry
                    var history = new TestHistory
                    {
                        UserId = App.CurrentUser.UserId,
                        TestName = randomTest.TestName,
                        DateTaken = DateTime.Now,
                        Score = 0 // Initial score
                    };
                    context.TestHistories.Add(history);
                    context.SaveChanges();
                }
            }
        }
    }
}
