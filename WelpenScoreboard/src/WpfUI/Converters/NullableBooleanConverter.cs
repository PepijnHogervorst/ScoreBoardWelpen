using System.Globalization;
using System.Windows.Data;

namespace WelpenScoreboard.WpfUI.Converters;

public class NullableBooleanConverter<T> : IValueConverter
{
    public NullableBooleanConverter(T trueValue, T falseValue, T nullValue)
    {
        True = trueValue;
        False = falseValue;
        Null = nullValue;
    }

    public T True { get; set; }
    public T False { get; set; }
    public T Null { get; set; }

    public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return Null;
        return value is bool boolVal && boolVal ? True : False;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is T boolVal && EqualityComparer<T>.Default.Equals(boolVal, True);
    }
}
