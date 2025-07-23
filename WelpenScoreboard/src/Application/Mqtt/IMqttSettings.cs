using System.Text.Json;

namespace WelpenScoreboard.Application.Mqtt;
public interface IMqttSettings
{
    JsonSerializerOptions SerializerOptions { get; }
}
