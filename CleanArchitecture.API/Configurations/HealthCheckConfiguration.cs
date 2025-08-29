using System.Text.Json;
using CleanArchitecture.API.HealthChecks;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitecture.API.Configurations;

public static class HealthCheckConfiguration
{
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(
                name: "database",
                tags: ["db", "ef", "postgresql"]
            )
            .AddNpgSql(
                configuration.GetConnectionString("Default")!,
                name: "postgresql",
                tags: ["db", "postgresql"]
            )
            .AddRedis(
                configuration.GetConnectionString("Redis")!,
                name: "redis",
                tags: ["cache", "redis"]
            )
            .AddCheck<ApplicationHealthCheck>(
                name: "application",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["app", "startup"]
            );

        services.AddHealthChecksUI(options =>
        {
            options.AddHealthCheckEndpoint("API", "/health");
            options.SetEvaluationTimeInSeconds(30);
            options.SetApiMaxActiveRequests(1);
            options.MaximumHistoryEntriesPerEndpoint(50);
        })
        .AddInMemoryStorage();

        return services;
    }

    public static IEndpointRouteBuilder MapHealthCheckEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = WriteHealthCheckResponse
        });

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("cache"),
            ResponseWriter = WriteHealthCheckResponse
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("app"),
            ResponseWriter = WriteHealthCheckResponse
        });

        endpoints.MapHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-ui-api";
        });

        return endpoints;
    }

    private static Task WriteHealthCheckResponse(
        HttpContext context,
        HealthReport report
    )
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.TotalMilliseconds,
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data,
            }),
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        return context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
    }
}
