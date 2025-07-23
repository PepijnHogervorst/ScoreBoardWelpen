namespace WelpenScoreboard.Application.Mqtt.Events;
public class MqttMessageReceivedEventArgs : EventArgs
{
    public string Topic { get; }
    public string Message { get; }
    public MqttMessageReceivedEventArgs(string topic, string message)
    {
        Topic = topic;
        Message = message;
    }
}
