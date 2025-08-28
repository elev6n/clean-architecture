using System.Text;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    [Obsolete]
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration
    )
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "CleanArchitecture:";
        });


        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
            {
                SchemaName = "hangfire" // Отдельная схема для Hangfire
            }));
        
        services.AddHangfireServer();

        // JWT  
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
                };
            });

        // Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddSingleton<IJwtTokenGenerator>(provider =>
            new JwtTokenGenerator(
                jwtSettings["SecretKey"]!,
                jwtSettings["Issuer"]!,
                jwtSettings["Audience"]!
            )
        );

        return services;
    }
}