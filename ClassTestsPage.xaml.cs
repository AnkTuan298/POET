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
using System.ComponentModel;

namespace POET
{
    public partial class ClassTestsPage : UserControl, INotifyPropertyChanged
    {
        private bool _hasClass;
        public event PropertyChangedEventHandler PropertyChanged;


        public ClassTestsPage()
        {
            InitializeComponent();
            DataContext = this;
            CheckClassEnrollment();
        }

        public bool HasClass
        {
            get => _hasClass;
            set
            {
                _hasClass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasClass)));
                if (_hasClass) LoadClassTests();
            }
        }

        public List<Test> Tests { get; private set; }

        private void CheckClassEnrollment()
        {
            using var context = new PoetContext();
            HasClass = context.Classes
                .Any(c => c.Students.Any(s => s.UserId == App.CurrentUser.UserId));
        }

        private void LoadClassTests()
        {
            using var context = new PoetContext();
            Tests = context.Tests
                .Include(t => t.Class)
                .ThenInclude(c => c.Students)
                .Where(t => t.Class.Students
                    .Any(s => s.UserId == App.CurrentUser.UserId))
                .ToList();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tests)));
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int testId)
            {
                using var context = new PoetContext();
                var test = context.Tests
                    .Include(t => t.Questions)
                    .Include(t => t.Level)
                    .First(t => t.TestId == testId);

                var mainWindow = Window.GetWindow(this) as UserMainWindow;
                mainWindow?.contentFrame.Navigate(new TestInfoPage(test));
            }
        }

        private void JoinClass_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            mainWindow?.contentFrame.Navigate(new JoinClassPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            mainWindow?.contentFrame.Navigate(new WelcomePage());
        }
    }
}
