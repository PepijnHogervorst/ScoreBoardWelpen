using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Mqtt.Workers;
/// <summary>
/// Worker that checks for status timeouts of the arduino.
/// </summary>
public interface IMqttAliveWorker : IBackgroundWorker
{
}
