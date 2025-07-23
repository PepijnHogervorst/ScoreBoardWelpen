using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.Infrastructure.Mqtt;
internal class MqttControl : IMqttControl
{
    private static readonly string[] _topics =
        [
            "Scoreboard/Status",
            "Scoreboard/Ready",
        ];

    private readonly IMqttBroker _broker;
    private readonly IMqttClient _client;

    public MqttControl(IMqttBroker broker, IMqttClient client)
    {
        _broker = broker;
        _client = client;
    }

    public async Task<bool> StartAsync()
    {
        await _broker.StartAsync();
        if (!await _client.ConnectAsync()) return false;
        foreach (string topic in _topics)
        {
            if (!await _client.SubscribeAsync(topic)) return false;
        }
        return true;
    }

    public async Task StopAsync()
    {
        await _client.CloseAsync();
        await _broker.StopAsync();
    }

    public IEnumerable<string> Topics => _topics;
}
