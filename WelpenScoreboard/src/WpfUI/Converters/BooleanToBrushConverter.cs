using System.Windows.Media;

namespace WelpenScoreboard.WpfUI.Converters;

public class BooleanToBrushConverter : BooleanConverter<Brush>
{
    public BooleanToBrushConverter() : base(Brushes.Green, Brushes.Gray)
    {
    }
}
