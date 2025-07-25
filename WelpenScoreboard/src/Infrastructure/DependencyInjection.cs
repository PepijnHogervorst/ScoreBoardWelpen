using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Domain.Common.Extensions;
using WelpenScoreboard.Domain.Common.Interfaces;
using WelpenScoreboard.Infrastructure.Persistence;

namespace WelpenScoreboard.Infrastructure;
public static class DependencyInjection
{
    private static readonly Assembly _infrastructureAssembly = typeof(DependencyInjection).Assembly;

    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext();
        services.AddMqtt();

        services.AddTypesFromAssemblies<IAlwaysActiveBackgroundWorker>(ServiceLifetime.Singleton, _infrastructureAssembly);
    }

    private static IServiceCollection AddMqtt(this IServiceCollection services)
    {
        services.AddSingleton<IMqttBroker, Mqtt.MqttNetBroker>();
        services.AddSingleton<IMqttClient, Mqtt.MqttNetClient>();
        services.AddSingleton<IMqttControl, Mqtt.MqttControl>();
        services.AddSingleton<IMqttSettings, Mqtt.MqttSettings>();
        services.AddSingleton<Application.Mqtt.Workers.IMqttAliveProgram, Mqtt.Workers.MqttAliveProgram>();
        services.AddSingleton<Application.Mqtt.Workers.IMqttAliveWorker, Mqtt.Workers.MqttAliveWorker>();
        services.AddTypesFromAssemblies<IMqttTopicParser>(ServiceLifetime.Singleton, _infrastructureAssembly);

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        Action<DbContextOptionsBuilder> configureDbContext = b =>
        {
            b.UseSqlite(GetSqliteConnection(),
                        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        };

        services.AddDbContext<ApplicationDbContext>(configureDbContext);
        services.AddScoped<DbContext>(s => s.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static SqliteConnection GetSqliteConnection()
    {
        var connectionBuilder = new SqliteConnectionStringBuilder()
        {
            DataSource = "",
        };

        return new(connectionBuilder.ToString());
    }
}
