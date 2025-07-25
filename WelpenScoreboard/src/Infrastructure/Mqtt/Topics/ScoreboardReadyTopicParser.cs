using WelpenScoreboard.Application.Led;
using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.Infrastructure.Mqtt.Topics;
internal class ScoreboardReadyTopicParser : IMqttTopicParser
{
    private readonly IMqttSettings _mqttSettings;
    private readonly ILedApplicationStore _ledStore;

    public string Topic => "Scoreboard/Ready";

    public ScoreboardReadyTopicParser(IMqttSettings mqttSettings,
                                      ILedApplicationStore ledStore)
    {
        _mqttSettings = mqttSettings;
        _ledStore = ledStore;
    }

    public Task ParseAsync(string json)
    {
        try
        {
            // Signal to the application that the scoreboard is ready
            // Don't worry about the JSON content, we just need to know that the Arduino is ready
            _ledStore.SignalThatArduinoIsReady();
            return Task.CompletedTask;
        }
        catch (Exception)
        {

        }
        return Task.CompletedTask;
    }
}
