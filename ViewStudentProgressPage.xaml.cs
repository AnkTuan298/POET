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
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using POET.Models;

namespace POET
{
    public partial class ViewStudentProgressPage : UserControl
    {
        private readonly PoetContext _context = new PoetContext();
        private List<StudentProgressCard> _allProgress;

        public ViewStudentProgressPage()
        {
            InitializeComponent();
            LoadProgress();
        }

        private void LoadProgress()
        {
            var teacherClass = _context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.Profile)
                .FirstOrDefault(c => c.TeacherId == App.CurrentUser.UserId);

            if (teacherClass == null) return;

            var testNames = _context.Tests
                .Where(t => t.ClassId == teacherClass.ClassId)
                .Select(t => t.TestName)
                .Distinct()
                .ToList();

            var progressCards = new List<StudentProgressCard>();

            foreach (var student in teacherClass.Students)
            {
                var displayName = student.Profile?.Name ?? student.Username;

                var scoreList = new List<string>();

                foreach (var testName in testNames)
                {
                    var latestAttempt = _context.TestHistories
                        .Where(h => h.UserId == student.UserId && h.TestName == testName)
                        .OrderByDescending(h => h.DateTaken)
                        .FirstOrDefault();

                    var scoreText = latestAttempt != null ? latestAttempt.Score.ToString() : "Not Attempted";
                    scoreList.Add($"{testName} : {scoreText}");
                }

                progressCards.Add(new StudentProgressCard
                {
                    StudentName = $"Student : {displayName}",
                    ScoreList = scoreList
                });
            }

            _allProgress = progressCards;
            progressListControl.ItemsSource = _allProgress;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = txtSearch.Text.Trim().ToLower();
            var mode = (cmbSearchMode.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                progressListControl.ItemsSource = _allProgress;
                return;
            }

            List<StudentProgressCard> filtered;

            if (mode == "By Name")
            {
                filtered = _allProgress
                    .Where(p => p.StudentName.ToLower().Contains(keyword))
                    .ToList();
            }
            else // By Test
            {
                filtered = _allProgress
                    .Select(p => new StudentProgressCard
                    {
                        StudentName = p.StudentName,
                        ScoreList = p.ScoreList
                            .Where(line => line.ToLower().Contains(keyword))
                            .ToList()
                    })
                    .Where(p => p.ScoreList.Any())
                    .ToList();
            }

            progressListControl.ItemsSource = filtered;
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this) as TeacherMainWindow;
            if (window != null)
            {
                window.contentFrame.Navigate(new StudentListPage());
            }
        }

    }

    public class StudentProgressCard
    {
        public string StudentName { get; set; }
        public List<string> ScoreList { get; set; }
    }
}


