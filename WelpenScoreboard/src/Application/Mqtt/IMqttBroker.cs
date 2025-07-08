
namespace WelpenScoreboard.Application.Mqtt;

public interface IMqttBroker
{
    Task StartAsync();
    Task StopAsync();
}
