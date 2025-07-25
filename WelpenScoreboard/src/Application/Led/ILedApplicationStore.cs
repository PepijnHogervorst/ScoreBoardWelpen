using WelpenScoreboard.Application.Mqtt.Json;

namespace WelpenScoreboard.Application.Led;
public interface ILedApplicationStore
{
    string Status { get; set; }
    int Brightness { get; set; }
    string ArduinoVersion { get; set; }
    DateTimeOffset LastUpdated { get; }
    bool IsConnected { get; set; }
    string IsConnectedText { get; }
    int GroupTurn { get; set; }

    event EventHandler<EventArgs>? OnArduinoReady;

    void SignalThatArduinoIsReady();
    void UpdateStatus(StatusData statusData);
}
