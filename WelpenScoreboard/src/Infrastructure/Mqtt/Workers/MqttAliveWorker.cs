using Microsoft.Extensions.Logging;
using WelpenScoreboard.Application.Common.Workers;
using WelpenScoreboard.Application.Mqtt.Workers;

namespace WelpenScoreboard.Infrastructure.Mqtt.Workers;
internal class MqttAliveWorker : BackgroundTimerDelegateWorker, IMqttAliveWorker
{
    public MqttAliveWorker(IMqttAliveProgram program, ILogger<MqttAliveWorker> logger) : base(program, logger)
    {
    }
}
