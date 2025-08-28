using CleanArchitecture.API.Common;
using CleanArchitecture.Application.Common.Behaviors;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Users.Commands.CreateUser;
using CleanArchitecture.Infrastructure;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Hellang.Middleware.ProblemDetails;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// OpenAPI
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Clean Architecture API";
    config.Version = "v1";
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

// Exception Handling
builder.Services.AddGlobalExceptionHandler();

// Hangfire
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Default"))
);

// Infrastructure
var connectionString = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddInfrastructure(connectionString, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.MapOpenApi();
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        DashboardTitle = "Clean Architecture Jobs",
        Authorization = [new HangfireAuthorizationFilter()]
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseProblemDetails();
app.MapControllers();

RecurringJob.AddOrUpdate<IDataCleanupService>(
    "cleanup-old-data",
    x => x.CleanupOldDataAsync(),
    "0 2 * * *"
);

app.Run();

// Фильтр авторизации для Dashboard
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // В продакшене нужно добавить настоящую авторизацию
        return true;
    }
}