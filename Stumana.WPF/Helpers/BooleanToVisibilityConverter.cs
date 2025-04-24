using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Stumana.WPF
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                // Return true if the visibility is visible, otherwise false                return visibility == Visibility.Visible;
            }

            return false;
        }
    }
}
