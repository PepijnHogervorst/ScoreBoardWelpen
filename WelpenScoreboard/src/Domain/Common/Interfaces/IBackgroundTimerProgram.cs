namespace WelpenScoreboard.Domain.Common.Interfaces;
/// <summary>
/// Interface for a background program that runs a task non stop
/// </summary>
public interface IBackgroundTimerProgram : IBackgroundProgram
{
    TimeSpan Interval { get; }
}
