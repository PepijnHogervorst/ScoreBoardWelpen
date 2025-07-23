using Microsoft.Extensions.Logging;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Common.Workers;
public abstract class BackgroundTimerProgram : BackgroundProgram, IBackgroundTimerProgram
{
    public TimeSpan Interval { get; }


    protected BackgroundTimerProgram(ILogger logger, TimeSpan interval, string? name = null)
        : base(logger, name)
    {
        Interval = interval;
    }
}
