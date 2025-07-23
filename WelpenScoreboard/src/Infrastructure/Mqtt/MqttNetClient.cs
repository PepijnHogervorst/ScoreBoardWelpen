using System.Text;
using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Application.Mqtt.Events;

namespace WelpenScoreboard.Infrastructure.Mqtt;
internal class MqttNetClient : IMqttClient
{
    private readonly MQTTnet.IMqttClient _client;
    private readonly MQTTnet.MqttClientFactory _factory;
    private Func<MqttMessageReceivedEventArgs, Task>? _funcOnMessageReceived;


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
             .WithTcpServer("127.0.0.1", 1883)
             .Build();
        var result = await _client.ConnectAsync(options, CancellationToken.None);
        return result.ResultCode == MQTTnet.MqttClientConnectResultCode.Success;
    }

    public void SetCallback(Func<MqttMessageReceivedEventArgs, Task> funcOnMessageReceived)
    {
        _funcOnMessageReceived = funcOnMessageReceived;
    }

    public async Task CloseAsync()
    {
        if (!_client.IsConnected) return;
        await _client.DisconnectAsync(new MQTTnet.MqttClientDisconnectOptionsBuilder()
            .WithReason(MQTTnet.MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build());
    }

    public async Task<bool> PublishAsync(string topic, string message)
    {
        if (!_client.IsConnected) return false;

        try
        {
            var result = await _client.PublishAsync(
                new MQTTnet.MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes(message))
                    .Build());
            return result.IsSuccess;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SubscribeAsync(string topic)
    {
        if (!_client.IsConnected) return false;

        try
        {
            var subscribeOptions = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();
            var result = await _client.SubscribeAsync(subscribeOptions);
            return result.Items.Count > 0 && result.Items.First().ResultCode == MQTTnet.MqttClientSubscribeResultCode.GrantedQoS0;
        }
        catch (Exception)
        {
            // Handle exceptions as needed
            return false;
        }
    }

    private async Task OnApplicationMessageReceivedAsync(MQTTnet.MqttApplicationMessageReceivedEventArgs arg)
    {
        if (_funcOnMessageReceived is null) return;
        await _funcOnMessageReceived(new(arg.ApplicationMessage.Topic, Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)));
    }
}
