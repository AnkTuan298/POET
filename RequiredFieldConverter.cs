using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace POET
{
    public class RequiredFieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string)
                ? new SolidColorBrush(Colors.LightPink)
                : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}