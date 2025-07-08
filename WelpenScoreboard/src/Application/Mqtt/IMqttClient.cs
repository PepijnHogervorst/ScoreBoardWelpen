
namespace WelpenScoreboard.Application.Mqtt;
public interface IMqttClient
{
    Task CloseAsync();
    Task<bool> ConnectAsync();
    Task PublishAsync(string topic, string message);
}
