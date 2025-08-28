using CleanArchitecture.API.Common;
using CleanArchitecture.Application.Common.Behaviors;
using CleanArchitecture.Application.Users.Commands.CreateUser;
using CleanArchitecture.Infrastructure;
using FluentValidation;
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
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

// Exception Handling
builder.Services.AddGlobalExceptionHandler();

// Infrastructure
var connectionString = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseProblemDetails();
app.MapControllers();

app.Run();
