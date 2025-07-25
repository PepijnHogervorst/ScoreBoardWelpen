using MQTTnet.Server;
using System.Diagnostics;
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

        var mqttServerFactory = new MqttServerFactory();
        var mqttServerOptions = mqttServerFactory.CreateServerOptionsBuilder()
                                                 .WithDefaultEndpoint()
                                                 .Build();

        try
        {
            _mqttServer = mqttServerFactory.CreateMqttServer(mqttServerOptions);
            await _mqttServer.StartAsync();
        }
        catch (Exception)
        {
            Debug.WriteLine("Failed to start MQTT broker..");
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
