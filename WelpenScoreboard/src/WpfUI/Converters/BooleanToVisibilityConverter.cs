using System.Windows;

namespace WelpenScoreboard.WpfUI.Converters;

public class BooleanToVisibilityConverter : BooleanConverter<Visibility>
{
    public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
    { }
}
