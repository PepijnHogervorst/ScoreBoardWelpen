using Microsoft.Extensions.Logging;
using WelpenScoreboard.Application.Common.Workers;

namespace WelpenScoreboard.Infrastructure.Mqtt.Workers;
internal class MqttAliveProgram : BackgroundTimerProgram
{
    public MqttAliveProgram(ILogger<MqttAliveProgram> logger)
        : base(logger, TimeSpan.FromMilliseconds(100), nameof(MqttAliveProgram))
    {

    }

    public override Task BackgroundTask(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
