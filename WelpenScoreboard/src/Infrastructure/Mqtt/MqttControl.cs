using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.Infrastructure.Mqtt;
internal class MqttControl : IMqttControl
{
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
        return await _client.ConnectAsync();
    }

    public async Task StopAsync()
    {
        await _client.CloseAsync();
        await _broker.StopAsync();
    }
}
