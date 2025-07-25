using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WelpenScoreboard.WpfUI.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public class StringToImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string valueString) return null;

        try
        {
            ImageSource image = BitmapFrame.Create(new Uri(valueString), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
            return image;
        }
        catch { return null; }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
