using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WelpenScoreboard.Application.Persistence;
using WelpenScoreboard.Domain.Entities;

namespace WelpenScoreboard.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ILogger<ApplicationDbContext> _logger;

    public DbSet<ApplicationSetting> Settings => Set<ApplicationSetting>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Person> Persons => Set<Person>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                ILogger<ApplicationDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(
            (eventId, level) => level >= LogLevel.Warning,
            (data) =>
            {
                _logger.Log(data.LogLevel, "[EF] {EventId}\n\t{Message}", data.ToString(), data.EventIdCode);
            });

        base.OnConfiguring(optionsBuilder);
    }
}
