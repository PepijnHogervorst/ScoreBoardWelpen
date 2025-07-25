namespace WelpenScoreboard.WpfUI.Converters;

public class BooleanToDoubleConverter : BooleanConverter<double>
{
    public BooleanToDoubleConverter() : base(0, 1)
    {
    }
}
