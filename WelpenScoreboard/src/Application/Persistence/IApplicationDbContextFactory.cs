using WelpenScoreboard.Application.Persistence;

namespace ScalableHMI.Application.Common.Interfaces;
/// <summary>
/// Interface for creating instances of IApplicationDbContext.
/// </summary>
public interface IApplicationDbContextFactory
{
    IApplicationDbContext CreateDbContext();
}
