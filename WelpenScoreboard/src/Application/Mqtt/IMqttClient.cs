using WelpenScoreboard.Application.Mqtt.Events;

namespace WelpenScoreboard.Application.Mqtt;
public interface IMqttClient
{
    Task CloseAsync();
    Task<bool> ConnectAsync();
    Task<bool> PublishAsync(string topic, string message);
    void SetCallback(Func<MqttMessageReceivedEventArgs, Task> funcOnMessageReceived);
    Task<bool> SubscribeAsync(string topic);
}
