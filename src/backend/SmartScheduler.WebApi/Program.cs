using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.Services;
using SmartScheduler.Infrastructure.ExternalServices;
using SmartScheduler.Infrastructure.Persistence;
using SmartScheduler.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add database context with connection string fallback logic
Console.WriteLine("STARLING: Attempting to load database connection string...");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["DATABASE_URL"]
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("STARLING: ERROR - No database connection string found!");
    throw new InvalidOperationException(
        "Database connection string not found. " +
        "Set ConnectionStrings:DefaultConnection, DATABASE_URL environment variable, " +
        "or provide a Render PostgreSQL database.");
}

Console.WriteLine($"STARLING: Connection string source detected, length: {connectionString.Length}");

// Render provides DATABASE_URL in format: postgres://user:pass@host:port/db
// Convert to Npgsql connection string format: Host=...;Database=...;Username=...;Password=...
// Only convert if it's in URI format, otherwise assume it's already in Npgsql format
if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
{
    Console.WriteLine("STARLING: Detected URI format, converting to Npgsql connection string format");

    try
    {
        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');

        connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        Console.WriteLine($"STARLING: Converted to Npgsql format (Host={host}, Port={port}, Database={database})");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"STARLING: ERROR converting URI to connection string: {ex.Message}");
        throw;
    }
}
else if (connectionString.Contains("Host="))
{
    Console.WriteLine("STARLING: Detected Npgsql format, using as-is");
}
else
{
    Console.WriteLine("STARLING: WARNING - Unknown connection string format!");
}

// Log connection info (mask password for both formats)
var maskedConnection = connectionString;
// Mask password in Npgsql format: Password=xxx;
maskedConnection = System.Text.RegularExpressions.Regex.Replace(
    maskedConnection,
    @"(Password=)([^;]+)(;?)",
    "$1****$3");
// Mask password in URI format: ://user:pass@
maskedConnection = System.Text.RegularExpressions.Regex.Replace(
    maskedConnection,
    @"(:\/\/[^:]+:)([^@]+)(@)",
    "$1****$3");
Console.WriteLine($"STARLING: Using connection string: {maskedConnection}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories
builder.Services.AddScoped<IContractorRepository, ContractorRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobTypeRepository, JobTypeRepository>();
builder.Services.AddScoped<IDomainEventLogRepository, DomainEventLogRepository>();

// Register domain services
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IDistanceService, SmartScheduler.Infrastructure.Services.DistanceService>();

// Register memory cache for distance caching
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 10000; // Limit to 10,000 cache entries (~1MB)
});

// Register distance calculation services
var apiKey = builder.Configuration["OPENROUTESERVICE_API_KEY"];
if (string.IsNullOrEmpty(apiKey))
{
    // Log warning but don't fail - allow app to start for demo purposes
    // Distance calculations will fall back to straight-line distance
    Console.WriteLine("WARNING: OPENROUTESERVICE_API_KEY not configured. Distance calculations will use fallback.");
    apiKey = "dummy-key-not-configured"; // Placeholder to allow startup
}

builder.Services.AddHttpClient<OpenRouteServiceClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("https://api.openrouteservice.org");
        client.Timeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddSingleton(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient(nameof(OpenRouteServiceClient));
    var logger = sp.GetRequiredService<ILogger<OpenRouteServiceClient>>();
    return new OpenRouteServiceClient(httpClient, logger, apiKey);
});

builder.Services.AddSingleton<DistanceCache>();
builder.Services.AddScoped<IDistanceCalculator, DistanceCalculator>();

// Register MediatR for CQRS pattern
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(SmartScheduler.Application.Commands.CreateContractorCommand).Assembly);
    // Register event handlers from WebApi layer (SignalR broadcast handler)
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Note: FluentValidation validators are already in Application layer
// They will be resolved through MediatR pipeline if needed

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add Problem Details for standardized error responses
builder.Services.AddProblemDetails();

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add CORS for frontend communication (must allow credentials for SignalR)
builder.Services.AddCors(options =>
{
    options.AddPolicy("SmartSchedulerCorsPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow any localhost origin
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Required for SignalR
        }
        else
        {
            // In production, use configured origins (check both formats, with hardcoded fallback)
            var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"]
                ?? Environment.GetEnvironmentVariable("CORS__AllowedOrigins")
                ?? "https://smartscheduler-frontend-ltq9.onrender.com"; // Hardcoded fallback for Render deployment

            policy.WithOrigins(allowedOrigins.Split(','))
                .AllowAnyHeader() // Allow all headers (SignalR needs X-Requested-With, X-SignalR-User-Agent, etc.)
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("X-Correlation-ID"); // Expose custom response headers
        }
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS (must be before other middleware to ensure headers on all responses)
app.UseCors("SmartSchedulerCorsPolicy");

// Add exception handler to ensure CORS headers on error responses
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred. Please try again later."
        });
    });
});

// Use Problem Details middleware
app.UseStatusCodePages();

// Map controllers
app.MapControllers()
    .RequireCors("SmartSchedulerCorsPolicy");

// Map SignalR hub
app.MapHub<SmartScheduler.WebApi.Hubs.SchedulingHub>("/hubs/scheduling")
    .RequireCors("SmartSchedulerCorsPolicy");

// Health check endpoint for container health monitoring
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Make Program accessible to integration tests
public partial class Program { }
