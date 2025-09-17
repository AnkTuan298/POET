using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using POET.Views;

namespace POET
{
    public partial class TestResultPage : UserControl
    {
        public TestResultPage(int score, int correctCount, string timeRemaining, List<QuestionResult> results)
        {
            InitializeComponent();

            // Process results for display
            var processedResults = results.Select(r => new QuestionResult
            {
                QuestionText = r.QuestionText,
                UserAnswer = string.IsNullOrEmpty(r.UserAnswer) ? "Not answered" : r.UserAnswer,
                CorrectAnswer = r.CorrectAnswer,
                IsCorrect = r.IsCorrect
            }).ToList();

            DataContext = new ResultViewModel
            {
                Score = score,
                CorrectCount = correctCount,
                TimeRemaining = timeRemaining,
                QuestionResults = processedResults
            };
        }

        private void BackToTests_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            mainWindow?.contentFrame.Navigate(new ClassTestsPage());
        }
    }

    public class QuestionResult
    {
        public string QuestionText { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class ResultViewModel
    {
        public int Score { get; set; }
        public int CorrectCount { get; set; }
        public string TimeRemaining { get; set; }
        public List<QuestionResult> QuestionResults { get; set; }
    }
}