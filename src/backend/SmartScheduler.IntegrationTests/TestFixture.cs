using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Infrastructure.Persistence;

namespace SmartScheduler.IntegrationTests;

/// <summary>
/// Test fixture using WebApplicationFactory for in-memory API testing.
/// Manages database lifecycle and provides HTTP client for integration tests.
/// </summary>
public class TestFixture : WebApplicationFactory<Program>
{
    private readonly string _databaseName;

    public TestFixture()
    {
        _databaseName = $"smartscheduler_test_{Guid.NewGuid()}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database (PostgreSQL with unique database name)
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    $"Host=localhost;Port=5432;Database={_databaseName};" +
                    $"Username=postgres;Password=postgres");
            });

            // Reduce logging noise during tests
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.UseEnvironment("Test");
    }

    /// <summary>
    /// Initializes the test database with schema and seed data.
    /// Creates a fresh database for each test run.
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        // Create database and apply migrations
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Cleans up the test database after test execution.
    /// </summary>
    public async Task CleanupDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        // Drop the test database
        await context.Database.EnsureDeletedAsync();
    }

    /// <summary>
    /// Gets a scoped service from the test server's service provider.
    /// </summary>
    public T GetService<T>() where T : notnull
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Seeds the database with job types for testing.
    /// </summary>
    public async Task SeedJobTypesAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        // Check if job types already exist
        if (!await context.JobTypes.AnyAsync())
        {
            var jobTypes = new[]
            {
                new JobType("Tile Installer"),
                new JobType("Carpet Installer"),
                new JobType("Hardwood Specialist")
            };

            context.JobTypes.AddRange(jobTypes);
            await context.SaveChangesAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Cleanup is handled by CleanupDatabaseAsync
        }
        base.Dispose(disposing);
    }
}
