using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WelpenScoreboard.Domain.Common.Extensions;
using WelpenScoreboard.WpfUI.ViewModels.Interfaces;

namespace WelpenScoreboard.WpfUI;

public static class DependencyInjection
{
    private readonly static Assembly _wpfAssembly = typeof(DependencyInjection).Assembly;

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<Views.MainWindow>();
        services.AddSingleton<ViewModels.MainViewModel>();
        services.RegisterAssemblyTypes<ITabViewModel>(ServiceLifetime.Singleton, _wpfAssembly);
    }
}
