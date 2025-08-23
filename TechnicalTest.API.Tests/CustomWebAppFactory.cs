using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using TechnicalTest.Data;

namespace TechnicalTest.API.Tests;

public sealed class CustomWebAppFactory : WebApplicationFactory<Program>, IDisposable
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Create a single in-memory SQLite connection for the app lifetime
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Build and migrate the DB
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            db.Database.Migrate();
        });
    }

    public void Dispose()
    {
        base.Dispose();
        _connection?.Dispose();
    }
}
