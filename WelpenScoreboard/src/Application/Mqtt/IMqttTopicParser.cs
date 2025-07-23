
namespace WelpenScoreboard.Application.Mqtt;
public interface IMqttTopicParser
{
    public string Topic { get; }

    Task ParseAsync(string json);
}
