

namespace WelpenScoreboard.Application.Mqtt;
public interface IMqttControl
{
    IEnumerable<string> Topics { get; }

    Task<bool> StartAsync();
    Task StopAsync();
}
