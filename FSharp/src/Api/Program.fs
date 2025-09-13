open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore
open CleanArchitecture.Application
open CleanArchitecture.Infrastructure.Repositories
open CleanArchitecture.Domain.RepositoryInterfaces
open CleanArchitecture.Infrastructure
open CleanArchitecture.Infrastructure.Database
open CleanArchitecture.Api.middleware
open CleanArchitecture.Api.Middleware

let builder = WebApplication.CreateBuilder()

// Добавляем сервисы
builder.Services.AddControllers() |> ignore
builder.Services.AddEndpointsApiExplorer() |> ignore

// Для Swagger в .NET 9 используем OpenAPI
builder.Services.AddOpenApi() |> ignore

// Настройка базы данных
// builder.Services.AddDbContext<AppDbContext>(fun options ->
//     options.UseNpgsql(builder.Configuration.GetConnectionString "DefaultConnection")
//     |> ignore)
// |> ignore

// builder.Services.AddDbContext<AppDbContext>(fun options ->
//     options.UseInMemoryDatabase "TestDatabase"
//     |> ignore)
// |> ignore

(addDbContext builder.Services, builder.Configuration) |> ignore

// Регистрация репозиториев
builder.Services.AddScoped<IUserRepository>(fun provider ->
    let dbContext = provider.GetRequiredService<AppDbContext>()
    UserRepository dbContext :> IUserRepository)
|> ignore

builder.Services.AddScoped<ITodoRepository>(fun provider ->
    let dbContext = provider.GetRequiredService<AppDbContext>()
    TodoRepository dbContext :> ITodoRepository)
|> ignore

// Регистрация MediatR
builder.Services.AddMediatR(fun config ->
    config.RegisterServicesFromAssembly(typeof<Users.GetUserByIdQuery>.Assembly)
    |> ignore)
|> ignore

let app = builder.Build()

try 
    use scope = app.Services.CreateScope()
    let dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>()
    dbContext.Database.Migrate()
with 
| ex -> printfn "Migration failed: %s" ex.Message

app.UseMiddleware<ExceptionHandlerMiddleware>() |> ignore
app.UseMiddleware<RequestLoggingMiddleware>() |> ignore
app.UseMiddleware<ValidationMiddleware>() |> ignore

// Конфигурация middleware
if app.Environment.IsDevelopment() then
    app.MapOpenApi() |> ignore

app.UseHttpsRedirection() |> ignore
app.UseAuthorization() |> ignore
app.MapControllers() |> ignore

app.Run()
