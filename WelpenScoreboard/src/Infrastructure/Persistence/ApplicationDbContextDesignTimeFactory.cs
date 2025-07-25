using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScalableHMI.Infrastructure.Services.Common;
using ScalableHMI.Infrastructure.Services.HMI;
using ScalableHMI.Infrastructure.Stores.HMI;
using WelpenScoreboard.Infrastructure.Persistence;

namespace ScalableHMI.Infrastructure.Persistence;
/// <summary>
/// Design time factory required to create database migrations (because of clean architecture project setup this is required)
/// </summary>
public class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionBuilder.UseSqlite(GetSqliteConnection());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        return new ApplicationDbContext(optionBuilder.Options, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    private static SqliteConnection GetSqliteConnection()
    {
        var connectionBuilder = new SqliteConnectionStringBuilder()
        {
            DataSource = ApplicationStartup.GetSqliteDataSourcePath(),
            Password = CreateDeviceIdService().UniqueComputerId
        };

        return new(connectionBuilder.ToString());
    }

    private static DeviceIdService CreateDeviceIdService() => new(
            new RegistryConfigurationStore(),
            new RegistryService());
}
