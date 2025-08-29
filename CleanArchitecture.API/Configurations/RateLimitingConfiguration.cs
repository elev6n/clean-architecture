using System.Threading.RateLimiting;

namespace CleanArchitecture.API.Configurations;

public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimitingServices(
        this IServiceCollection services
    )
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString()!,
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }
                )
            );

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response
                    .WriteAsync("Too many requests. Please try again later.", token);
            };
        });

        return services;
    }
}
