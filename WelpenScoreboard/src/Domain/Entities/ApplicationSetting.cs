namespace WelpenScoreboard.Domain.Entities;
/// <summary>
/// Holds application settings such as the amount of groups used.
/// </summary>
public class ApplicationSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
