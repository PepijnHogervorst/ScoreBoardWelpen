using Microsoft.Extensions.DependencyInjection;
using WelpenScoreboard.Application.Mqtt;

namespace WelpenScoreboard.Infrastructure;
public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMqttBroker, Mqtt.MqttNetBroker>();
    }
}
