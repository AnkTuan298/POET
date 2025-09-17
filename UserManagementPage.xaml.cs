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
using Microsoft.EntityFrameworkCore;
using POET.Views;

namespace POET
{
    public partial class UserManagementPage : UserControl
    {
        public UserManagementPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var context = new PoetContext())
            {
                usersGrid.ItemsSource = context.Users
                    .Include(u => u.Profile)
                    .ToList();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var userId = (int)((Button)sender).Tag;

            // Prevent deleting current user
            if (userId == App.CurrentUser.UserId)
            {
                new CustomMessageBox("You cannot delete your own account!").ShowDialog();
                return;
            }

            // Check user role before showing confirmation
            using (var checkContext = new PoetContext())
            {
                var userToDelete = checkContext.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UserId == userId);

                if (userToDelete?.Role?.Equals("admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    new CustomMessageBox("Admin users cannot be deleted.").ShowDialog();
                    return;
                }
            }

            // Proceed with confirmation for non-admin users
            bool confirm = YesNoMessageBox.Show(
                "Delete this user permanently?",
                "Confirm Delete"
            );

            if (confirm)
            {
                using (var context = new PoetContext())
                {
                    var user = context.Users
                        .Include(u => u.TestHistories)
                        .Include(u => u.ClassesNavigation)
                        .First(u => u.UserId == userId);

                    // Remove student from classes
                    foreach (var classEntry in user.ClassesNavigation.ToList())
                    {
                        classEntry.Students.Remove(user);
                    }

                    context.Users.Remove(user);
                    context.SaveChanges();
                    LoadUsers();
                }
            }
        }

        private void UpgradeRole_Click(object sender, RoutedEventArgs e)
        {
            var userId = (int)((Button)sender).Tag;
            var dialog = new RoleUpgradeDialog();

            if (dialog.ShowDialog() == true)
            {
                using (var context = new PoetContext())
                {
                    var user = context.Users.Find(userId);
                    user.Role = dialog.SelectedRole;
                    context.SaveChanges();
                    LoadUsers();
                }
            }
        }
    }
}
