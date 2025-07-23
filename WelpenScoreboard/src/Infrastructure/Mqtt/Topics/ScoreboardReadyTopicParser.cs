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

    public async Task ParseAsync(string json)
    {
        try
        {

        }
        catch (Exception)
        {

        }
    }
}
