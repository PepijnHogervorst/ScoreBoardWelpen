using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WelpenScoreboard.Domain.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAssemblyTypes<T>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        var interfaces = assemblies
            .SelectMany(o => o.DefinedTypes
                .Where(x => x.IsInterface || x.IsAbstract)
                .Where(x => x != typeof(T))
                .Where(x => typeof(T).IsAssignableFrom(x))
            );

        foreach (var @interface in interfaces)
        {
            var types = assemblies
                .SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsClass && !x.IsAbstract)
                    .Where(x => @interface.IsAssignableFrom(x))
                );

            foreach (var type in types)
            {
                services.Add(new ServiceDescriptor(
                    typeof(T),
                    type,
                    lifetime)
                );
            }
        }

        return services;
    }
}
