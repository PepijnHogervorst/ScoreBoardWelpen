using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.WpfUI.Services;
internal class Startup
{
    private readonly IMqttControl _mqttControl;

    public Startup(IMqttControl mqttControl)
    {
        _mqttControl = mqttControl;
    }

    public async Task InitializeAsync()
    {
        // Start the MQTT broker and client
        await StartMqtt();
    }

    private async Task StartMqtt()
    {
        if (await _mqttControl.StartAsync()) return;
        Console.WriteLine("Failed to start MQTT broker or client.");
    }
}
