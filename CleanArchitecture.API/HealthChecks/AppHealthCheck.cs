using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitecture.API.HealthChecks;

public class ApplicationHealthCheck : IHealthCheck
{
    private readonly ILogger<ApplicationHealthCheck> _logger;

    public ApplicationHealthCheck(ILogger<ApplicationHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            _logger.LogInformation("Application health check executed");

            return Task.FromResult(HealthCheckResult.Healthy(
                "Application is running",
                new Dictionary<string, object>
                {
                    { "version", "1.0.0" },
                    {
                        "environment",
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                            ?? "Development"
                    }
                }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Application is unhealthy", ex));
        }
    }
}
