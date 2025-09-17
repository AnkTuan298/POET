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
using Microsoft.EntityFrameworkCore;
using POET.Models;
using System.Linq;
using POET.Views;

namespace POET
{
    public partial class ParticipantListPage : UserControl
    {
        private readonly int _classId;

        public ParticipantListPage(int classId)
        {
            InitializeComponent();
            _classId = classId;
            LoadParticipants();
        }

        private void LoadParticipants()
        {
            using var context = new PoetContext();
            var selectedClass = context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.Profile)
                .First(c => c.ClassId == _classId);

            participantsGrid.ItemsSource = selectedClass.Students.ToList();
        }

        private void DeleteParticipant_Click(object sender, RoutedEventArgs e)
        {
            var studentId = (int)((Button)sender).Tag;

            bool confirm = YesNoMessageBox.Show(
                "Remove this student from the class?",
                "Confirm Removal"
            );

            if (confirm)
            {
                using var context = new PoetContext();
                var selectedClass = context.Classes
                    .Include(c => c.Students)
                    .First(c => c.ClassId == _classId);

                var student = selectedClass.Students
                    .First(s => s.UserId == studentId);

                selectedClass.Students.Remove(student);
                context.SaveChanges();
                LoadParticipants();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as AdminMainWindow;
            mainWindow?.contentFrame.Navigate(new ClassManagementPage());
        }
    }
}
