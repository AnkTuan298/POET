using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using POET.Models;

namespace POET.Views
{
    public partial class ViewStudentProgressPage : UserControl
    {
        public ViewStudentProgressPage()
        {
            InitializeComponent();
            LoadStudentProgress();
        }

        private void LoadStudentProgress()
        {
            using var context = new PoetContext();

            // Get teacher's class
            var teacherClass = context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.Profile)
                .Include(c => c.Tests)
                .FirstOrDefault(c => c.TeacherId == App.CurrentUser.UserId);

            var progressList = new List<StudentTestProgress>();

            if (teacherClass != null && teacherClass.Students.Count > 0 && teacherClass.Tests.Count > 0)
            {
                foreach (var student in teacherClass.Students)
                {
                    foreach (var test in teacherClass.Tests)
                    {
                        var latestHistory = context.TestHistories
                            .Where(th => th.UserId == student.UserId && th.TestName == test.TestName)
                            .OrderByDescending(th => th.DateTaken)
                            .FirstOrDefault();

                        progressList.Add(new StudentTestProgress
                        {
                            StudentName = student.Profile?.Name ?? "",
                            Username = student.Username,
                            Email = student.Profile?.Email ?? "",
                            TestName = test.TestName,
                            LatestScore = latestHistory?.Score ?? 0, // 0 if not taken
                            DateTaken = latestHistory?.DateTaken.ToString("g") ?? "" // empty if not taken
                        });
                    }
                }
            }

            progressGrid.ItemsSource = progressList;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as TeacherMainWindow;
            mainWindow?.contentFrame.Navigate(new StudentListPage());
        }

        private class StudentTestProgress
        {
            public string StudentName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string TestName { get; set; }
            public int LatestScore { get; set; }
            public string DateTaken { get; set; }
        }
    }
}