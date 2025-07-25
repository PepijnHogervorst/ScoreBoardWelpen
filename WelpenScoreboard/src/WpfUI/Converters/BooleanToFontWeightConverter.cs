using System.Windows;

namespace WelpenScoreboard.WpfUI.Converters;

internal class BooleanToFontWeightConverter : BooleanConverter<FontWeight>
{
    public BooleanToFontWeightConverter() : base(FontWeights.Bold, FontWeights.Normal)
    {

    }
}
