using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace POET
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCorrect)
            {
                // Handle "Not answered" case
                if (parameter is string param && param == "NotAnswered")
                    return Brushes.Gray;

                return isCorrect ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}