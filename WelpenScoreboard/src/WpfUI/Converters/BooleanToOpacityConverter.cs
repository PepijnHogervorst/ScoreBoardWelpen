namespace WelpenScoreboard.WpfUI.Converters;

public class BooleanToOpacityConverter : BooleanConverter<double>
{
    public BooleanToOpacityConverter() : base(1.0, 0.6)
    {
    }
}
