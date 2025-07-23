using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Domain.Common.Extensions;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Infrastructure;
public static class DependencyInjection
{
    private static readonly Assembly _infrastructureAssembly = typeof(DependencyInjection).Assembly;

    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddMqtt();

        services.RegisterAssemblyTypes<IAlwaysActiveBackgroundWorker>(ServiceLifetime.Singleton, _infrastructureAssembly);
    }

    private static IServiceCollection AddMqtt(this IServiceCollection services)
    {
        services.AddSingleton<IMqttBroker, Mqtt.MqttNetBroker>();
        services.AddSingleton<IMqttClient, Mqtt.MqttNetClient>();
        services.AddSingleton<IMqttControl, Mqtt.MqttControl>();
        services.AddSingleton<IMqttSettings, Mqtt.MqttSettings>();

        return services;
    }
}
