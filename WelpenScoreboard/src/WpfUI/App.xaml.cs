using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using WelpenScoreboard.WpfUI.Services;
using WelpenScoreboard.WpfUI.Views;

namespace WelpenScoreboard.WpfUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<App> _logger;

    public App()
    {
        _configuration = BuildConfig();

        ServiceCollection serviceCollection = new();
        serviceCollection.ConfigureServices(_configuration);

        _serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });

        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<App>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        var startup = _serviceProvider.GetRequiredService<Startup>();
        await startup.InitializeAsync();

        var window = _serviceProvider.GetRequiredService<MainWindow>();
        window.Show();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        // Dispose of services if needed
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private static IConfiguration BuildConfig() => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
}
