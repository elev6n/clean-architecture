using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;

namespace CleanArchitecture.API.Configurations;

public static class HangfireConfiguration
{
    public static IServiceCollection AddHangfireServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire",
                };
            })
        );

        services.AddHangfireServer();

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboard(
        this IApplicationBuilder app
    )
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            DashboardTitle = "Clean Architecture Jobs",
            Authorization = [new HangfireAuthorizationFilter()],
        });

        return app;
    }
}

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context) => true;
}
