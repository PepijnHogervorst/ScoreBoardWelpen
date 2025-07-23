using System.Text.Json;
using WelpenScoreboard.Application.Led;
using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Application.Mqtt.Json;

namespace WelpenScoreboard.Infrastructure.Mqtt.Topics;
internal class ScoreboardStatusTopicParser : IMqttTopicParser
{
    private readonly IMqttSettings _mqttSettings;
    private readonly ILedApplicationStore _ledStore;

    public string Topic => "Scoreboard/Status";

    public ScoreboardStatusTopicParser(IMqttSettings mqttSettings,
                                       ILedApplicationStore ledStore)
    {
        _mqttSettings = mqttSettings;
        _ledStore = ledStore;
    }

    public async Task ParseAsync(string json)
    {
        try
        {
            var statusData = JsonSerializer.Deserialize<StatusData>(json, _mqttSettings.SerializerOptions);
            if (statusData is null) return;

            _ledStore.UpdateStatus(statusData);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
