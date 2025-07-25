using System.Globalization;
using System.Windows.Data;

namespace WelpenScoreboard.WpfUI.Converters;

public class BooleanConverter<T> : IValueConverter
{
    public BooleanConverter(T trueValue, T falseValue)
    {
        True = trueValue;
        False = falseValue;
    }

    public T True { get; set; }
    public T False { get; set; }

    public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolVal && boolVal ? True : False;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is T boolVal && EqualityComparer<T>.Default.Equals(boolVal, True);
    }
}
