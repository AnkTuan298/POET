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
    /// <summary>
    /// Interaction logic for JoinClassPage.xaml
    /// </summary>
    public partial class JoinClassPage : UserControl
    {
        public JoinClassPage()
        {
            InitializeComponent();
        }

        private void JoinClass_Click(object sender, RoutedEventArgs e)
        {
            var classCode = txtClassCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(classCode))
            {
                new CustomMessageBox("Please enter a class code").ShowDialog();
                return;
            }

            using var context = new PoetContext();
            var targetClass = context.Classes
                .FirstOrDefault(c => c.ClassCode == classCode);

            if (targetClass == null)
            {
                new CustomMessageBox("Invalid class code").ShowDialog();
                return;
            }

            var user = context.Users
                .Include(u => u.ClassesNavigation)
                .First(u => u.UserId == App.CurrentUser.UserId);

            if (user.ClassesNavigation.Any(c => c.ClassId == targetClass.ClassId))
            {
                new CustomMessageBox("You're already in this class").ShowDialog();
                return;
            }

            user.ClassesNavigation.Add(targetClass);
            context.SaveChanges();
            new CustomMessageBox("Successfully joined class!").ShowDialog();
        }
    }
}
