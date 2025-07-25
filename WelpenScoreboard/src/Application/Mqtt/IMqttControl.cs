

namespace WelpenScoreboard.Application.Mqtt;
/// <summary>
/// Main interface to start and stop the MQTT system (broker and client).
/// </summary>
public interface IMqttControl
{
    List<string> Topics { get; }

    Task<bool> StartAsync();
    Task StopAsync();
}
