namespace WelpenScoreboard.Application.Mqtt.Json;
public record ScoreboardData
{
    public int Group { get; set; }
    public int Points { get; set; }
}
