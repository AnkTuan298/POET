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
using System.Windows.Threading;
using POET.Views;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace POET
{
    public partial class TestPage : UserControl
    {
        private readonly DispatcherTimer _timer;
        private TimeSpan _timeRemaining;
        private readonly Test _currentTest;
        private readonly List<Question> _questions;
        private readonly User _currentUser;
        private int _violationCount;
        private bool _isWarningShown;
        private DispatcherTimer _focusTimer;
        private IntPtr _windowHandle;

        // Keyboard hook fields
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static bool _altPressed = false;

        public TestPage(Test selectedTest, User currentUser)
        {
            InitializeComponent();
            _currentTest = selectedTest;
            _currentUser = currentUser;

            // Load questions
            using var context = new PoetContext();
            _questions = context.Questions
                .Where(q => q.TestId == _currentTest.TestId)
                .ToList();

            QuestionsItemsControl.ItemsSource = _questions;

            // Setup timer
            _timeRemaining = TimeSpan.FromMinutes(15);
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            Unloaded += TestPage_Unloaded;

            // Enter test mode (force fullscreen and lock Alt+Tab)
            Loaded += (s, e) =>
            {
                var mainWindow = Window.GetWindow(this) as UserMainWindow;
                mainWindow?.EnterTestMode();
                ForceFullScreen(mainWindow);
                _hookID = SetHook(_proc);
            };

            // Exit test mode and release hook
            Unloaded += (s, e) =>
            {
                var mainWindow = Window.GetWindow(this) as UserMainWindow;
                mainWindow?.ExitTestMode();
                ReleaseHook();
            };
        }

        private void TestPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _focusTimer?.Stop();
            _focusTimer = null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
            txtTimer.Text = _timeRemaining.ToString(@"mm\:ss");

            if (_timeRemaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                SubmitTest();
            }
        }

        private void SubmitTest_Click(object sender, RoutedEventArgs e)
        {
            SubmitTest();
        }

        private void SubmitTest()
        {
            _focusTimer?.Stop();
            _timer.Stop();

            // Exit test mode and release hook
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            mainWindow?.ExitTestMode();
            ReleaseHook();

            // Calculate score
            int score = CalculateScore();
            string timeRemaining = _timeRemaining.ToString(@"mm\:ss");

            // Create test history record
            try
            {
                using (var context = new PoetContext())
                {
                    var testHistory = new TestHistory
                    {
                        UserId = App.CurrentUser.UserId,
                        TestName = _currentTest.TestName,
                        Score = score,
                        DateTaken = DateTime.Now
                    };

                    context.TestHistories.Add(testHistory);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                new CustomMessageBox($"Error saving test history: {ex.Message}").ShowDialog();
            }

            // Navigate to results page
            if (mainWindow != null)
            {
                mainWindow.btnProfile.Visibility = Visibility.Visible;
                mainWindow.btnTakeTest.Visibility = Visibility.Visible;
                mainWindow.btnJoinClass.Visibility = Visibility.Visible;
                mainWindow.txtPoetLogo.Visibility = Visibility.Visible;
                mainWindow.btnClass.Visibility = Visibility.Visible;
                mainWindow.btnRoleUpgrade.Visibility = Visibility.Visible;
                mainWindow.btnLogout.Visibility = Visibility.Visible;
            }

            mainWindow?.contentFrame.Navigate(new TestResultPage(
                score,
                _questions.Count(q => q.UserAnswer == q.CorrectOption),
                timeRemaining,
                _questions.Select(q => new QuestionResult
                {
                    QuestionText = q.QuestionText,
                    UserAnswer = q.UserAnswer,
                    CorrectAnswer = q.CorrectOption,
                    IsCorrect = q.UserAnswer == q.CorrectOption
                }).ToList()
            ));
        }

        private string GetOptionText(Question question, string option)
        {
            // Handle null/empty explicitly
            if (string.IsNullOrEmpty(option))
                return "Not answered";

            return option.ToUpper() switch
            {
                "A" => question.OptionA,
                "B" => question.OptionB,
                "C" => question.OptionC,
                "D" => question.OptionD,
                _ => "Not answered"  // Fallback for invalid values
            };
        }

        private int CalculateScore()
        {
            int correctCount = 0;
            foreach (var question in _questions)
            {
                if (question.UserAnswer == question.CorrectOption)
                {
                    correctCount++;
                }
            }
            return (int)Math.Round((double)correctCount / _questions.Count * 100);
        }

        /// <summary>
        /// Forces the main window to fullscreen and disables resizing, window controls, and sets Topmost.
        /// </summary>
        private void ForceFullScreen(Window mainWindow)
        {
            if (mainWindow != null)
            {
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.ResizeMode = ResizeMode.NoResize;
                mainWindow.WindowState = WindowState.Maximized;
                mainWindow.Topmost = true;
            }
        }

        // Keyboard hook implementation
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static void ReleaseHook()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int VK_TAB = 0x09;
        private const int VK_MENU = 0x12; // Alt key

        /// <summary>
        /// Blocks Alt+Tab and Windows key during test mode.
        /// </summary>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Block Alt+Tab
                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    if (vkCode == VK_MENU)
                        _altPressed = true;
                    if (_altPressed && vkCode == VK_TAB)
                        return (IntPtr)1; // Block Alt+Tab
                    // Block Windows key (Left/Right)
                    if (vkCode == 0x5B || vkCode == 0x5C)
                        return (IntPtr)1;
                }
                else
                {
                    if (vkCode == VK_MENU)
                        _altPressed = false;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}