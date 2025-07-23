using Microsoft.Extensions.DependencyInjection;
using ScalableHMI.Domain.Common.Interfaces;
using System.Reflection;
using WelpenScoreboard.Application.Mqtt;
using WelpenScoreboard.Domain.Common.Extensions;

namespace WelpenScoreboard.Infrastructure;
public static class DependencyInjection
{
    private static readonly Assembly _infrastructureAssembly = typeof(DependencyInjection).Assembly;

    public static void AddInfrastructure(this IServiceCollection services)
        services.RegisterAssemblyTypes<IAlwaysActiveBackgroundWorker>(ServiceLifetime.Singleton, _infrastructureAssembly);
    {
        services.AddSingleton<IMqttBroker, Mqtt.MqttNetBroker>();
    }
}
