using WelpenScoreboard.Application.Led;
using WelpenScoreboard.Application.Mqtt.Json;

namespace WelpenScoreboard.WpfUI.Stores;
internal class LedApplicationStore : ViewModelBase, ILedApplicationStore
{
    private string _status = "Not Connected";
    private int _brightness = 0;
    private string _arduinoVersion = "Unknown";
    private bool _isConnected;

    public string Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public int Brightness
    {
        get => _brightness;
        set
        {
            if (_brightness == value) return;
            _brightness = value;
            OnPropertyChanged(nameof(Brightness));
        }
    }

    public string ArduinoVersion
    {
        get => _arduinoVersion;
        set
        {
            if (_arduinoVersion == value) return;
            _arduinoVersion = value;
            OnPropertyChanged(nameof(ArduinoVersion));
        }
    }

    public DateTimeOffset LastUpdated { get; private set; } = DateTimeOffset.Now;

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnPropertyChanged(nameof(IsConnected));
        }
    }

    public void UpdateStatus(StatusData statusData)
    {
        Status = statusData.State;
        Brightness = statusData.Brightness;
        ArduinoVersion = statusData.Version;
        LastUpdated = DateTimeOffset.Now;
        IsConnected = true;
    }
}
