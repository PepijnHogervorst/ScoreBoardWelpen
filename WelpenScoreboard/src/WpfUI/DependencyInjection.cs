using Microsoft.Extensions.DependencyInjection;

namespace WelpenScoreboard.WpfUI;

public static class DependencyInjection
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
    }
}
