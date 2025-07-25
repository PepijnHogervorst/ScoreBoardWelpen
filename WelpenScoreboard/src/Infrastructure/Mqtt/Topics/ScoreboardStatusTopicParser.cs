using Microsoft.Extensions.Logging;
using System.Text.Json;
using WelpenScoreboard.Application.Led;
using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Application.Mqtt.Json;

namespace WelpenScoreboard.Infrastructure.Mqtt.Topics;
internal class ScoreboardStatusTopicParser : IMqttTopicParser
{
    private readonly IMqttSettings _mqttSettings;
    private readonly ILedApplicationStore _ledStore;
    private readonly ILogger<ScoreboardStatusTopicParser> _logger;

    private bool _hasLoggedError = false;

    public string Topic => "Scoreboard/Status";

    public ScoreboardStatusTopicParser(IMqttSettings mqttSettings,
                                       ILedApplicationStore ledStore,
                                       ILogger<ScoreboardStatusTopicParser> logger)
    {
        _mqttSettings = mqttSettings;
        _ledStore = ledStore;
        _logger = logger;
    }

    public Task ParseAsync(string json)
    {
        try
        {
            var statusData = JsonSerializer.Deserialize<StatusData>(json, _mqttSettings.SerializerOptions);
            if (statusData is null) return Task.CompletedTask;

            _ledStore.UpdateStatus(statusData);
        }
        catch (Exception ex)
        {
            if (_hasLoggedError) return Task.CompletedTask;
            _hasLoggedError = true;
            _logger.LogError(ex, "Unable to parse status data from Arduino..");
        }
        return Task.CompletedTask;
    }
}
