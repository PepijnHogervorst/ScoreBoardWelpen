using WelpenScoreboard.Application.Mqtt.Json;

namespace WelpenScoreboard.Application.Led;
public interface ILedApplicationStore
{
    string Status { get; set; }
    int Brightness { get; set; }
    string ArduinoVersion { get; set; }

    void UpdateStatus(StatusData statusData);
}
