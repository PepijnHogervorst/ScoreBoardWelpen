using WelpenScoreboard.Domain.Enums;

namespace WelpenScoreboard.WpfUI.Converters;

public class NullableBooleanToLedStateConverter : NullableBooleanConverter<LedState>
{
    public NullableBooleanToLedStateConverter()
        : base(LedState.Enabled, LedState.Error, LedState.Disabled)
    {
    }
}
