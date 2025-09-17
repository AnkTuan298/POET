using System.Configuration;
using System.Data;
using System.Windows;
using POET.Models;

namespace POET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
    }

}
