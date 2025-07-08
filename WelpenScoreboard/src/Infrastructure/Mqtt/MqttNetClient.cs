using System.Text;
using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.Infrastructure.Mqtt;
internal class MqttNetClient : IMqttClient
{
    private readonly MQTTnet.IMqttClient _client;
    private readonly MQTTnet.MqttClientFactory _factory;

    public MqttNetClient()
    {
        _factory = new MQTTnet.MqttClientFactory();
        _client = _factory.CreateMqttClient();
        _client.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
    }

    ~MqttNetClient()
    {
        _client.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
        _client.Dispose();
    }

    public async Task<bool> ConnectAsync()
    {
        if (_client.IsConnected) return true;

        var options = new MQTTnet.MqttClientOptionsBuilder()
             .WithClientId("WelpenScoreboardClient")
             .WithTcpServer("localhost", 1883)
             .Build();
        var result = await _client.ConnectAsync(options, CancellationToken.None);
        return result.ResultCode == MQTTnet.MqttClientConnectResultCode.Success;
    }

    public async Task CloseAsync()
    {
        if (!_client.IsConnected) return;
        await _client.DisconnectAsync(new MQTTnet.MqttClientDisconnectOptionsBuilder()
            .WithReason(MQTTnet.MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build());
    }

    public async Task PublishAsync(string topic, string message)
    {
        if (!_client.IsConnected) return;

        try
        {
            await _client.PublishAsync(
                new MQTTnet.MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes(message))
                    .Build());

        }
        catch (Exception)
        {

        }
    }

    public async Task SubscribeAsync(string topic)
    {
        if (!_client.IsConnected) return;

        try
        {
            var subscribeOptions = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();
            await _client.SubscribeAsync(subscribeOptions);
        }
        catch (Exception)
        {
            // Handle exceptions as needed
        }
    }

    private Task OnApplicationMessageReceivedAsync(MQTTnet.MqttApplicationMessageReceivedEventArgs arg)
    {
        throw new NotImplementedException();
    }
}
