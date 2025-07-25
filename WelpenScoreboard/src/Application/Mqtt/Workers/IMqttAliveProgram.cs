using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Mqtt.Workers;
/// <summary>
/// Tracks if the arduino is alive by sending a status message every second.
/// </summary>
public interface IMqttAliveProgram : IBackgroundTimerProgram
{
}
