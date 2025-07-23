using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Application.Mqtt.Events;

namespace WelpenScoreboard.Infrastructure.Led;
internal class LedProgram
{
    private readonly IMqttClient _mqttClient;

    public LedProgram(IMqttClient mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public void Start()
    {
        //_mqttClient.OnMessageReceived += OnMqttMessageReceived;
    }

    public void Stop()
    {
        //_mqttClient.OnMessageReceived -= OnMqttMessageReceived;
    }

    private void OnMqttMessageReceived(object? sender, MqttMessageReceivedEventArgs e)
    {
        //switch (switch_on)
        //{
        //    default:
        //}
    }
}
