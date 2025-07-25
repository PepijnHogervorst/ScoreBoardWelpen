using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScalableHMI.Application.Common.Interfaces;
using WelpenScoreboard.Application.Persistence;

namespace ScalableHMI.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IApplicationDbContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContextFactory(Action<DbContextOptionsBuilder> configureDbContext, IMediator mediator,
                                       ILogger<ApplicationDbContext> logger)
    {
        _configureDbContext = configureDbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public IApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();

        _configureDbContext(options);

        return new ApplicationDbContext(options.Options, _mediator, _logger);
    }
}
