using POET.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using POET.Views;

namespace POET
{
    public partial class TestQuestionEditorPage : UserControl
    {
        private readonly int _testId;
        private readonly PoetContext _context = new PoetContext();

        public TestQuestionEditorPage(int testId)
        {
            InitializeComponent();
            _testId = testId;
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            var test = _context.Tests
                .Include(t => t.Questions)
                .First(t => t.TestId == _testId);

            foreach (var question in test.Questions)
            {
                question.CorrectOption = question.CorrectOption?.ToUpper();
            }

            questionsItems.ItemsSource = test.Questions.ToList();
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            var newQuestion = new Question
            {
                TestId = _testId,
                QuestionText = "New Question (Edit Me)",
                OptionA = "Option A",
                OptionB = "Option B",
                OptionC = "Option C",
                OptionD = "Option D",
                CorrectOption = "A"
            };

            _context.Questions.Add(newQuestion);
            _context.SaveChanges();
            LoadQuestions();
        }

        private void ImportQuestions_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    var newQuestions = new List<Question>();
                    var errorLog = new StringBuilder();
                    var successCount = 0;

                    foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        var parts = line.Split(new[] { "||" }, StringSplitOptions.None);

                        if (parts.Length != 6)
                        {
                            errorLog.AppendLine($"Invalid format: {line}");
                            continue;
                        }

                        var question = new Question
                        {
                            TestId = _testId,
                            QuestionText = parts[0].Trim(),
                            OptionA = parts[1].Trim(),
                            OptionB = parts[2].Trim(),
                            OptionC = parts[3].Trim(),
                            OptionD = parts[4].Trim(),
                            CorrectOption = parts[5].Trim().ToUpper()
                        };

                        if (ValidateQuestion(question, line, errorLog))
                        {
                            newQuestions.Add(question);
                            successCount++;
                        }
                    }

                    if (newQuestions.Any())
                    {
                        _context.Questions.AddRange(newQuestions);
                        _context.SaveChanges();
                        LoadQuestions();
                    }

                    ShowImportResult(successCount, errorLog);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Import failed: {ex.Message}");
                }
            }
        }

        private bool ValidateQuestion(Question question, string line, StringBuilder errorLog)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionText))
            {
                errorLog.AppendLine($"Missing question text: {line}");
                return false;
            }

            if (!new[] { "A", "B", "C", "D" }.Contains(question.CorrectOption))
            {
                errorLog.AppendLine($"Invalid correct answer '{question.CorrectOption}': {line}");
                return false;
            }

            return true;
        }

        private void ShowImportResult(int successCount, StringBuilder errorLog)
        {
            var message = new StringBuilder()
                .AppendLine($"Successfully imported {successCount} questions")
                .AppendLine()
                .AppendLine("Errors:");

            message.Append(errorLog.Length == 0 ? "None" : errorLog.ToString());

            new ScrollableMessageBox(message.ToString(), "Import Results").ShowDialog();
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            var questionId = (int)((Button)sender).Tag;
            var question = _context.Questions.Find(questionId);

            if (MessageBox.Show("Delete this question?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Questions.Remove(question);
                _context.SaveChanges();
                LoadQuestions();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("All changes saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            if (mainWindow != null)
            {
                // Navigate back to test management
                mainWindow.contentFrame.Navigate(new TestManagementPage());
               
            }
            else
            {
                new CustomMessageBox("Navigation failed").ShowDialog();
            }
        }

    }
}