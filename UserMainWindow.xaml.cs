using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using POET.Models;
using System.Windows.Input;

namespace POET.Views
{
    public partial class UserMainWindow : Window
    {
        public User CurrentUser { get; private set; }
        private bool _isNavigating;
        private bool _isClassMenuOpen = false;
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;
        private ResizeMode _previousResizeMode;
        private bool _previousTopmost; // <-- Add this line

        public UserMainWindow()
        {
            InitializeComponent();
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Session expired. Please login again.");
                this.Close();
                return;
            }
            InitializeNavigation();
            ShowWelcomePage();

        }

        private void InitializeNavigation()
        {
            contentFrame.Navigating += async (sender, e) =>
            {
                if (_isNavigating) return;
                _isNavigating = true;

                // Fade out animation
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2));
                contentFrame.BeginAnimation(OpacityProperty, fadeOut);
                await Task.Delay(200);
            };

            contentFrame.Navigated += async (sender, e) =>
            {
                // Fade in animation
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
                contentFrame.BeginAnimation(OpacityProperty, fadeIn);
                _isNavigating = false;
            };
        }

        private void ShowWelcomePage()
        {
            var welcomePage = new WelcomePage();
            welcomePage.txtUsername.Text = App.CurrentUser.Username;
            contentFrame.Navigate(welcomePage);
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isNavigating) return;

            var button = (Button)sender;
            ResetNavigationButtons();
            button.Background = Brushes.LightGray;

            switch (button.Name)
            {
                case "btnProfile":
                    contentFrame.Navigate(new StudentProfilePage());
                    break;
                case "btnTakeTest":
                    contentFrame.Navigate(new TakeTestPage());
                    break;
                case "btnRoleUpgrade":
                    contentFrame.Navigate(new RoleUpgradePage());
                    break;
            }
            classSubMenu.Visibility = Visibility.Collapsed;
        }

        private void Class_Click(object sender, RoutedEventArgs e)
        {
            _isClassMenuOpen = !_isClassMenuOpen;
            classSubMenu.Visibility = _isClassMenuOpen ? Visibility.Visible : Visibility.Collapsed;
            ResetNavigationButtons();
        }


        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            bool result = YesNoMessageBox.Show("Are you sure you want to log out?", "Logout");
            if (result)
            {
                new LoginWindow().Show();
                this.Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
                Close();
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void RoleUpgrade_Click(object sender, RoutedEventArgs e)
        {
            ResetNavigationButtons();
            classSubMenu.Visibility = Visibility.Collapsed;
            if (_isNavigating) return;
            contentFrame.Navigate(new RoleUpgradePage());

            btnRoleUpgrade.Background = Brushes.LightGray;
            
        }

        private void PoetLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isNavigating) return;
            ShowWelcomePage();
            btnProfile.ClearValue(BackgroundProperty);
            btnTakeTest.ClearValue(BackgroundProperty);
            btnJoinClass.ClearValue(BackgroundProperty);
            ResetNavigationButtons();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Optional: Hover effects
        private void WindowControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE81123"));
            ((TextBlock)((Button)sender).Content).Foreground = Brushes.White;
        }

        private void WindowControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = Brushes.Transparent;
            ((TextBlock)((Button)sender).Content).Foreground = (Brush)new BrushConverter().ConvertFrom("#FF444444");
        }

        private void ClassSubMenu_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ResetNavigationButtons();
            button.Background = Brushes.LightGray;

            switch (button.Name)
            {
                case "btnYourClass":
                    contentFrame.Navigate(new YourClassPage());
                    break;
                case "btnClassTests":
                    contentFrame.Navigate(new ClassTestsPage());
                    break;
                case "btnJoinClass":
                    contentFrame.Navigate(new JoinClassPage());
                    break;
            }
            _isClassMenuOpen = false;
        }

        public void EnterTestMode()
        {
            _previousWindowState = this.WindowState;
            _previousWindowStyle = this.WindowStyle;
            _previousResizeMode = this.ResizeMode;
            _previousTopmost = this.Topmost; // <-- Save previous Topmost

            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true; // <-- Set Topmost

            SidebarPanel.Visibility = Visibility.Collapsed;
        }

        public void ExitTestMode()
        {
            this.WindowState = _previousWindowState;
            this.WindowStyle = _previousWindowStyle;
            this.ResizeMode = _previousResizeMode;
            this.Topmost = _previousTopmost; // <-- Restore Topmost

            SidebarPanel.Visibility = Visibility.Visible;
        }



        public void ResetNavigationButtons()
        {
            btnProfile.ClearValue(BackgroundProperty);
            btnTakeTest.ClearValue(BackgroundProperty);
            btnJoinClass.ClearValue(BackgroundProperty);
            btnRoleUpgrade.ClearValue(BackgroundProperty);
            btnLogout.ClearValue(BackgroundProperty);
            btnYourClass.ClearValue(BackgroundProperty);
            btnClassTests.ClearValue(BackgroundProperty);
        }

    }
}