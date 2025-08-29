using CleanArchitecture.API.Common;
using CleanArchitecture.API.Configurations;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);
var postgresCS = builder.Configuration.GetConnectionString("Default")!;

// Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Clean Architecture API";
    config.Version = "v1";
});

// Exception Handling
builder.Services.AddGlobalExceptionHandler();

// Configurations
builder.Services.AddHealthCheckServices(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddRateLimitingServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.UseHangfireDashboard();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRateLimiter();
app.UseProblemDetails();

// Endpoints
app.MapControllers();
app.MapHealthCheckEndpoints();

app.Run();
