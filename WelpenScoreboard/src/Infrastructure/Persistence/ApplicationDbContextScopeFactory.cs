using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WelpenScoreboard.Infrastructure.Persistence;

namespace ScalableHMI.Infrastructure.Persistence;

public class ApplicationDbContextScopeFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContextScopeFactory(DbContextOptions<ApplicationDbContext> options, IMediator mediator,
                                            ILogger<ApplicationDbContext> logger)
    {
        _options = options;
        _mediator = mediator;
        _logger = logger;
    }

    public ApplicationDbContext CreateDbContext() => new(_options, _mediator, _logger);
}
