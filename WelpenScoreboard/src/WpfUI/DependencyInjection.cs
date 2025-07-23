using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using WelpenScoreboard.Domain.Common.Extensions;
using WelpenScoreboard.Infrastructure;
using WelpenScoreboard.WpfUI.ViewModels.Interfaces;

namespace WelpenScoreboard.WpfUI;

public static class DependencyInjection
{
    private readonly static Assembly _wpfAssembly = typeof(DependencyInjection).Assembly;

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddApplicationLogging(services, configuration);

        services.AddInfrastructure();

        services.AddServices();
        services.AddSingleton<Views.MainWindow>();
        services.AddSingleton<ViewModels.MainViewModel>();
        services.RegisterAssemblyTypes<ITabViewModel>(ServiceLifetime.Singleton, _wpfAssembly);
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<Services.Startup>();

        return services;
    }

    private static IServiceCollection AddApplicationLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(builder => builder.AddSerilog(dispose: true));
        return services;
    }
}
