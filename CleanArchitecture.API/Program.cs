using CleanArchitecture.Application.Handlers;
using CleanArchitecture.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateUserCommandHandler).Assembly
    )
);

// Infrastructure
var connectionString = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
