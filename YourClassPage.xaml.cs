using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using POET.Models;
using POET.Views;

namespace POET
{
    public partial class YourClassPage : UserControl, INotifyPropertyChanged
    {
        private Class _userClass;
        private bool _hasClass;

        public event PropertyChangedEventHandler PropertyChanged;

        public YourClassPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadClassInfo();
        }

        private void LoadClassInfo()
        {
            using var context = new PoetContext();
            _userClass = context.Classes
                .Include(c => c.Teacher)
                .FirstOrDefault(c => c.Students.Any(s => s.UserId == App.CurrentUser.UserId));

            HasClass = _userClass != null;

            // Cập nhật giao diện
            OnPropertyChanged(nameof(ClassName));
            OnPropertyChanged(nameof(TeacherName));
            OnPropertyChanged(nameof(FormattedClass));
            OnPropertyChanged(nameof(FormattedTeacher));
        }

        public bool HasClass
        {
            get => _hasClass;
            set
            {
                _hasClass = value;
                OnPropertyChanged(nameof(HasClass));
            }
        }

        public string ClassName => _userClass?.ClassName ?? "";
        public string TeacherName => _userClass?.Teacher?.Username ?? "";

        public string FormattedClass => $"Class : {ClassName}";
        public string FormattedTeacher => $"Teacher : {TeacherName}";

        private void LeaveClass_Click(object sender, RoutedEventArgs e)
        {
            if (_userClass == null) return;

            bool confirm = YesNoMessageBox.Show(
                "Leave this class? You'll lose access to its tests.",
                "Confirm Leave");

            if (confirm)
            {
                using var context = new PoetContext();
                var user = context.Users.Find(App.CurrentUser.UserId);
                var classToLeave = context.Classes
                    .Include(c => c.Students)
                    .First(c => c.ClassId == _userClass.ClassId);

                classToLeave.Students.Remove(user);
                context.SaveChanges();

                // Cập nhật lại UI
                _userClass = null;
                HasClass = false;
                OnPropertyChanged(nameof(ClassName));
                OnPropertyChanged(nameof(TeacherName));
                OnPropertyChanged(nameof(FormattedClass));
                OnPropertyChanged(nameof(FormattedTeacher));
            }
        }

        private void BtnMaterial_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as UserMainWindow;
            if (mainWindow != null)
            {
                mainWindow.contentFrame.Navigate(new ClassMaterialPage());
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
