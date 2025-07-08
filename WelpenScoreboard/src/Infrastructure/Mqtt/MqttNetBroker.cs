using MQTTnet.Server;
using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.Infrastructure.Mqtt;

public class MqttNetBroker : IMqttBroker
{
    private MqttServer? _mqttServer = null;

    ~MqttNetBroker()
    {
        _mqttServer?.Dispose();
    }

    public async Task StartAsync()
    {
        if (_mqttServer != null) return;

        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpointPort(1883)
            .Build();

        try
        {
            _mqttServer = new MqttServerFactory().CreateMqttServer(mqttServerOptions);
            await _mqttServer.StartAsync();
        }
        catch (Exception)
        {

        }
    }

    public async Task StopAsync()
    {
        if (_mqttServer == null) return;

        try
        {
            await _mqttServer.StopAsync();
            _mqttServer.Dispose();
            _mqttServer = null;
        }
        catch (Exception)
        {

        }
    }
}
