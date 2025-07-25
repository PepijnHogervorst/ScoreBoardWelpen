using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WelpenScoreboard.Domain.Entities;

namespace WelpenScoreboard.Application.Persistence;

public interface IApplicationDbContext : IDisposable
{
    DbSet<Person> Persons { get; }
    DbSet<Group> Groups { get; }
    DbSet<ApplicationSetting> Settings { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
