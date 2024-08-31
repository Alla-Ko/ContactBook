using System.Globalization;
using System.Windows.Data;
namespace ContactBook;
public class IntToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? string.Empty : ((int)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int result;
        if (int.TryParse(value as string, out result))
        {
            return result;
        }
        return 0; // або інше значення за замовчуванням
    }
}
