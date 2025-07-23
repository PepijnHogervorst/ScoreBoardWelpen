namespace WelpenScoreboard.Domain.Common.Interfaces;
/// <summary>
/// Exactly same as <see cref="IBackgroundWorker"/> but every implementation
/// is added to the DI container at startup (using assembly scanning) 
/// and directly run after app initialization 
/// </summary>
public interface IAlwaysActiveBackgroundWorker : IBackgroundWorker
{
}
