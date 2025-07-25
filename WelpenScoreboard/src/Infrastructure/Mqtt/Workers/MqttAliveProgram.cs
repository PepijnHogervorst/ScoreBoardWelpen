using Microsoft.Extensions.Logging;
using WelpenScoreboard.Application.Common.Workers;
using WelpenScoreboard.Application.Led;
using WelpenScoreboard.Application.Mqtt.Workers;

namespace WelpenScoreboard.Infrastructure.Mqtt.Workers;
internal class MqttAliveProgram : BackgroundTimerProgram, IMqttAliveProgram
{
    private readonly ILedApplicationStore _ledApplicationStore;
    private readonly TimeProvider _timeProvider;

    public MqttAliveProgram(ILogger<MqttAliveProgram> logger,
                            ILedApplicationStore ledApplicationStore,
                            TimeProvider timeProvider)
        : base(logger, TimeSpan.FromMilliseconds(100), nameof(MqttAliveProgram))
    {
        _ledApplicationStore = ledApplicationStore;
        _timeProvider = timeProvider;
    }

    public override Task BackgroundTask(CancellationToken token)
    {
        if (_timeProvider.GetLocalNow() <= _ledApplicationStore.LastUpdated + TimeSpan.FromSeconds(5)) return Task.CompletedTask;

        _ledApplicationStore.IsConnected = false;
        _ledApplicationStore.Status = "Not Connected";
        _ledApplicationStore.Brightness = 0;
        _ledApplicationStore.ArduinoVersion = "Unknown";

        return Task.CompletedTask;
    }
}
