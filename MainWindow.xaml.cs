using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using POET.Models;

namespace POET.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WelcomeScreenTransition();
        }

        private void WelcomeScreenTransition()
        {
            LoginScreen.Opacity = 0;

            // Set initial position of WelcomeScreen
            TranslateTransform welcomeTransform = new TranslateTransform();
            WelcomeScreen.RenderTransform = welcomeTransform;

            // Slide up animation
            DoubleAnimation slideUp = new DoubleAnimation
            {
                From = 0,
                To = -190,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
                BeginTime = TimeSpan.FromSeconds(1)
            };

            // Change text after sliding up
            slideUp.Completed += (s, e) => ChangeWelcomeText();

            // Apply slide-up animation
            welcomeTransform.BeginAnimation(TranslateTransform.YProperty, slideUp);
        }

        private void ChangeWelcomeText()
        {
            if (WelcomeText == null) return;


            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOut.Completed += (s, e) =>
            {

                WelcomeText.Text = "Pro Online English Test";

                DoubleAnimation fadeInText = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                fadeInText.Completed += (s2, e2) => ShowLoginScreen();
                WelcomeText.BeginAnimation(UIElement.OpacityProperty, fadeInText);
            };

            WelcomeText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void ShowLoginScreen()
        {

            DoubleAnimation fadeInLogin = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            LoginScreen.BeginAnimation(UIElement.OpacityProperty, fadeInLogin);
        }


        private void SlideToScreen(Grid fromScreen, Grid toScreen, bool clearInputs = true)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));

            fadeOut.Completed += (s, e) =>
            {
                fromScreen.Visibility = Visibility.Collapsed;
                toScreen.Visibility = Visibility.Visible;
                toScreen.BeginAnimation(OpacityProperty, fadeIn);

                if (clearInputs)
                {
                    ResetInputs();
                }
            };


            fromScreen.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            SlideToScreen(LoginScreen, ForgotPasswordScreen);
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            SlideToScreen(ForgotPasswordScreen, LoginScreen);
            SlideToScreen(RegisterScreen, LoginScreen);
        }
        private void ShowRegisterScreen(object sender, MouseButtonEventArgs e) 
        {
            SlideToScreen(LoginScreen, RegisterScreen);
        }

        //LOGIN PART
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false; // Disable button immediately

            using (var context = new PoetContext())
            {
                string username = txtUsername.Text;
                string password = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    new CustomMessageBox("All fields must be filled.").ShowDialog();
                    btnLogin.IsEnabled = true; // Re-enable if failed
                    return;
                }

                var user = context.Users
                    .FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    App.CurrentUser = user;

                    StartLoginSuccessTransition(() =>
                    {
                        // Role-based window navigation
                        switch (user.Role?.ToLower())
                        {
                            case "admin":
                                new AdminMainWindow().Show();
                                break;
                            case "teacher":
                                new TeacherMainWindow().Show();
                                break;
                            default:
                                new UserMainWindow().Show();
                                break;
                        }
                        this.Close();
                    });
                }
                else
                {
                    new CustomMessageBox("Invalid username or password.").ShowDialog();
                    btnLogin.IsEnabled = true; // Re-enable if failed
                }
            }
        }


        //5 Hour just for a Success Notification, dont touch
        private void StartLoginSuccessTransition(Action onCompleted)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOut.Completed += (s, _) =>
            {
                LoginScreen.Visibility = Visibility.Collapsed;


                SuccessMessage.Visibility = Visibility.Visible;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                var scaleAnim = new DoubleAnimation(1.2, 1, TimeSpan.FromSeconds(0.5));

                SuccessMessage.BeginAnimation(OpacityProperty, fadeIn);
                ((ScaleTransform)SuccessMessage.Children[0].RenderTransform)
                    .BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                ((ScaleTransform)SuccessMessage.Children[0].RenderTransform)
                    .BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);


                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += (s, _) =>
                {
                    timer.Stop();
                    onCompleted?.Invoke();
                };
                timer.Start();
            };

            LoginScreen.BeginAnimation(OpacityProperty, fadeOut);
        }


        //CHANGE PASSWORD PART
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string username = txtResetUsername.Text;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                new CustomMessageBox("All fields must be filled.").ShowDialog();
                return;
            }

            if (newPassword != confirmPassword)
            {
                new CustomMessageBox("Passwords do not match.").ShowDialog();
                return;
            }

            using (var context = new PoetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    new CustomMessageBox("Username not found.").ShowDialog();
                    return;
                }

                user.Password = newPassword;
                context.SaveChanges();

                new CustomMessageBox("Password changed successfully!").ShowDialog();
            }

            SlideToScreen(ForgotPasswordScreen, LoginScreen);
        }


        //REGISTER PART
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new PoetContext())
            {
                string username = txtRegisterUsername.Text;
                string password = txtRegisterPassword.Password;
                string confirmPassword = txtConfirmRegisterPassword.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    new CustomMessageBox("All fields must be filled.").ShowDialog();
                    return;
                }

                if (password != confirmPassword)
                {
                    new CustomMessageBox("Passwords do not match.").ShowDialog();
                    return;
                }

                if (context.Users.Any(u => u.Username == username))
                {
                    new CustomMessageBox("Username already exists.").ShowDialog();
                    return;
                }

                var newUser = new User
                {
                    Username = username,
                    Password = password,
                    Role = "Student"
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                new CustomMessageBox("Registration successful!").ShowDialog();
            }
            SlideToScreen(RegisterScreen, LoginScreen);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            if (LoginScreen.IsVisible && !string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Password))
            {
                Login_Click(btnLogin, null);
            }
            else if (RegisterScreen.IsVisible && !string.IsNullOrEmpty(txtRegisterUsername.Text))
            {
                Register_Click(btnRegister, null);
            }
            else if (ForgotPasswordScreen.IsVisible && !string.IsNullOrEmpty(txtResetUsername.Text))
            {
                ChangePassword_Click(btnChangePassword, null);
            }
        }

        //CLEANING FUNCTION
        private void ResetInputs()
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtRegisterUsername.Text = "";
            txtRegisterPassword.Password = "";
            txtConfirmRegisterPassword.Password = "";
            txtResetUsername.Text = "";
            txtNewPassword.Password = "";
            txtConfirmPassword.Password = "";

            FocusManager.SetFocusedElement(this, this);
            Keyboard.ClearFocus();
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseButtonBackground.Fill = Brushes.Red;
            CloseButtonIcon.Foreground = Brushes.White;
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButtonBackground.Fill = Brushes.Transparent;
            CloseButtonIcon.Foreground = Brushes.Black;
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }


        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtPasswordPlaceholder.Visibility = string.IsNullOrEmpty(txtPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtRegisterUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRegisterUsernamePlaceholder.Visibility =
                string.IsNullOrEmpty(txtRegisterUsername.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void txtRegisterPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtRegisterPasswordPlaceholder.Visibility = string.IsNullOrEmpty(txtRegisterPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtConfirmRegisterPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtConfirmRegisterPasswordPlaceholder.Visibility = string.IsNullOrEmpty(txtConfirmRegisterPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtRegisterUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRegisterUsername.Text)) txtRegisterUsernamePlaceholder.Visibility = Visibility.Visible;
        }

        private void txtRegisterUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            txtRegisterUsernamePlaceholder.Visibility = string.IsNullOrEmpty(txtRegisterUsername.Text) ? Visibility.Visible : Visibility.Collapsed;
        }



        private void txtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtNewPasswordPlaceholder.Visibility = string.IsNullOrEmpty(txtNewPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(txtConfirmPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }



        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsernamePlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsernamePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                txtUsernamePlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsernamePlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtUsernamePlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void txtResetUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtResetUsernamePlaceholder != null)
                txtResetUsernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void txtResetUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtResetUsername.Text))
                txtResetUsernamePlaceholder.Visibility = Visibility.Visible;
        }

        private void txtResetUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtResetUsernamePlaceholder.Visibility = string.IsNullOrEmpty(txtResetUsername.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtNewPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewPasswordPlaceholder != null)
                txtNewPasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void txtNewPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNewPassword.Password))
                txtNewPasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void txtConfirmPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtConfirmPasswordPlaceholder != null)
                txtConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void txtConfirmPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtConfirmPassword.Password))
                txtConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
        }
    }
}