using POET.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using POET.Views;

namespace POET
{
    public partial class AdminMainWindow : Window
    {
        private bool _isManagementOpen = false;

        public AdminMainWindow()
        {
            InitializeComponent();

            // Verify admin role on startup
            if (App.CurrentUser?.Role?.ToLower() != "admin")
            {
                MessageBox.Show("Unauthorized access");
                this.Close();
                return;
            }

            InitializeNavigation();
            ShowWelcomePage();
        }

        private void InitializeNavigation()
        {
            contentFrame.Navigating += (sender, e) =>
            {
                if (contentFrame.Content != null)
                    contentFrame.NavigationService.RemoveBackEntry();
            };
        }

        private void ShowWelcomePage()
        {
            contentFrame.Navigate(new AdminWelcomePage(App.CurrentUser.Username));
        }

        private void Management_Click(object sender, RoutedEventArgs e)
        {
            _isManagementOpen = !_isManagementOpen;
            subMenuPanel.Visibility = _isManagementOpen ? Visibility.Visible : Visibility.Collapsed;
            ShowWelcomePage();
            ResetButtonStyles();
        }

        private void SubMenu_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ResetButtonStyles();
            button.Background = Brushes.LightGray;

            switch (button.Name)
            {
                case "btnUsers":
                    contentFrame.Navigate(new UserManagementPage());
                    break;
                case "btnClasses":
                    contentFrame.Navigate(new ClassManagementPage());
                   break;
                case "btnTests":
                    contentFrame.Navigate(new TestManagementPage());
                    break;
            }
        }

        private void ResetButtonStyles()
        {
            btnUsers.ClearValue(BackgroundProperty);
            btnClasses.ClearValue(BackgroundProperty);
            btnTests.ClearValue(BackgroundProperty);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            new LoginWindow().Show();
            this.Close();
        }

        // Window Controls
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Hover Effects
        private void WindowControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE81123"));
                if (btn.Content is TextBlock textBlock)
                    textBlock.Foreground = Brushes.White;
            }
        }

        private void WindowControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Background = Brushes.Transparent;
                if (btn.Content is TextBlock textBlock)
                    textBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF444444");
            }
        }

        // Navigation Button Hover
        private void NavButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button btn)
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEEEEEE"));
        }

        private void NavButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Background.ToString() != "#FFEEEEEE")
                btn.Background = Brushes.Transparent;
        }
    }
}