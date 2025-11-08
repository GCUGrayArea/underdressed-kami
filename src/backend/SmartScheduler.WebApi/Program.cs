using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.Services;
using SmartScheduler.Infrastructure.ExternalServices;
using SmartScheduler.Infrastructure.Persistence;
using SmartScheduler.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IContractorRepository, ContractorRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobTypeRepository, JobTypeRepository>();

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
var apiKey = builder.Configuration["OPENROUTESERVICE_API_KEY"]
    ?? throw new InvalidOperationException("OPENROUTESERVICE_API_KEY not configured");

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
    cfg.RegisterServicesFromAssembly(
        typeof(SmartScheduler.Application.Commands.CreateContractorCommand).Assembly));

// Note: FluentValidation validators are already in Application layer
// They will be resolved through MediatR pipeline if needed

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add Problem Details for standardized error responses
builder.Services.AddProblemDetails();

// Add CORS for frontend communication
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                builder.Configuration["CORS:AllowedOrigins"]
                    ?? "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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

// Enable CORS
app.UseCors();

// Use Problem Details middleware
app.UseStatusCodePages();

// Map controllers
app.MapControllers();

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
