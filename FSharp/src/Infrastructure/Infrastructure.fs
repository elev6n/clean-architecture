namespace CleanArchitecture.Infrastructure

open System
open Microsoft.EntityFrameworkCore
open CleanArchitecture.Domain.Common
open CleanArchitecture.Domain.User
open CleanArchitecture.Domain.Todo

module Converters =
    open Microsoft.EntityFrameworkCore.Storage.ValueConversion

    type EntityIdConverter() =
        inherit ValueConverter<EntityId, Guid>((fun (EntityId v) -> v), (fun v -> EntityId v))

    type EmailConverter() =
        inherit ValueConverter<Email, string>((fun (Email v) -> v), (fun v -> Email v))

    type UsernameConverter() =
        inherit ValueConverter<Username, string>((fun (Username v) -> v), (fun v -> Username v))

    type TodoStatusConverter() =
        inherit
            ValueConverter<TodoStatus, string>(
                (fun status ->
                    match status with
                    | Pending -> "Pending"
                    | InProgress -> "InProgress"
                    | Completed -> "Completed"
                    | Archived -> "Archived"),
                (fun str ->
                    match str with
                    | "Pending" -> Pending
                    | "InProgress" -> InProgress
                    | "Completed" -> Completed
                    | "Archived" -> Archived
                    | _ -> Pending)
            )

    // Конвертер для string option
    type OptionToStringConverter() =
        inherit
            ValueConverter<string option, string>(
                (fun opt ->
                    match opt with
                    | Some value -> value
                    | None -> null),
                (fun str -> if String.IsNullOrEmpty str then None else Some str)
            )

open Converters

type AppDbContext(options: DbContextOptions<AppDbContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable users: DbSet<User>

    member this.Users
        with get () = this.users
        and set v = this.users <- v

    [<DefaultValue>]
    val mutable todos: DbSet<Todo>

    member this.Todos
        with get () = this.todos
        and set v = this.todos <- v

    override _.ConfigureConventions(configurationBuilder: ModelConfigurationBuilder) : unit =
        configurationBuilder.Properties<EntityId>().HaveConversion<EntityIdConverter>()
        |> ignore

        configurationBuilder.Properties<Email>().HaveConversion<EmailConverter>()
        |> ignore

        configurationBuilder.Properties<Username>().HaveConversion<UsernameConverter>()
        |> ignore

        configurationBuilder.Properties<TodoStatus>().HaveConversion<TodoStatusConverter>()
        |> ignore

        configurationBuilder.Properties<Option<string>>().HaveConversion<OptionToStringConverter>()
        |> ignore

    override _.OnModelCreating(modelBuilder: ModelBuilder) =
        modelBuilder.Entity<User>(fun entity -> entity.HasKey(fun u -> u.Id :> obj) |> ignore)
        |> ignore

        modelBuilder.Entity<Todo>(fun entity -> entity.HasKey(fun t -> t.Id :> obj) |> ignore)
        |> ignore

module Database =
    open Microsoft.Extensions.Configuration
    open Microsoft.Extensions.DependencyInjection

    let addDbContext (services: IServiceCollection) (configuration: IConfiguration) =
        services.AddDbContext<AppDbContext>(fun options ->
            options.UseNpgsql(configuration.GetConnectionString "DefaultConnection")
            |> ignore)
        |> ignore

        services

module Repositories =
    open System.Linq
    open CleanArchitecture.Domain.RepositoryInterfaces

    type UserRepository(dbContext: AppDbContext) =
        interface IUserRepository with
            member _.GetById(EntityId id) =
                async {
                    let! user = dbContext.Users.FindAsync(id).AsTask() |> Async.AwaitTask
                    return Option.ofObj user
                }

            member _.GetByEmail(Email email) =
                async {
                    let! user =
                        dbContext.Users.FirstOrDefaultAsync(fun u ->
                            match u.Email with
                            | Email e -> e = email)
                        |> Async.AwaitTask

                    return Option.ofObj user
                }

            member _.Create request =
                async {
                    let user: User =
                        { Id = EntityId(Guid.NewGuid())
                          Email = request.Email
                          Username = request.Username
                          CreatedAt = DateTime.UtcNow
                          UpdatedAt = DateTime.UtcNow }

                    dbContext.Users.Add user |> ignore
                    do! dbContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore
                    return user
                }

            member _.GetAll() =
                async {
                    let! users = dbContext.Users.ToListAsync() |> Async.AwaitTask
                    return users |> List.ofSeq
                }

    type TodoRepository(dbContext: AppDbContext) =
        interface ITodoRepository with
            member _.GetById(EntityId id) =
                async {
                    let! todo = dbContext.Todos.FindAsync(id).AsTask() |> Async.AwaitTask
                    return Option.ofObj todo
                }

            member _.GetByUserId(EntityId userId) =
                async {
                    let! todos =
                        dbContext.Todos
                            .Where(fun t ->
                                match t.UserId with
                                | EntityId guid -> guid = userId)
                            .ToListAsync()
                        |> Async.AwaitTask

                    return todos |> List.ofSeq
                }

            member _.Create request =
                async {
                    let todo: Todo =
                        { Id = EntityId(Guid.NewGuid())
                          Title = request.Title
                          Description = request.Description
                          Status = Pending
                          UserId = request.UserId
                          CreatedAt = DateTime.UtcNow
                          UpdatedAt = DateTime.UtcNow }

                    dbContext.Todos.Add todo |> ignore
                    do! dbContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore
                    return todo
                }

            member _.Update request =
                async {
                    let! todoOpt = dbContext.Todos.FindAsync(request.Id).AsTask() |> Async.AwaitTask

                    match Option.ofObj todoOpt with
                    | None -> return None
                    | Some todo ->
                        let updatedTodo =
                            { todo with
                                Title = request.Title |> Option.defaultValue todo.Title
                                Description =
                                    match request.Description with
                                    | Some desc -> Some desc
                                    | None -> todo.Description
                                Status = request.Status |> Option.defaultValue todo.Status
                                UpdatedAt = DateTime.UtcNow }

                        dbContext.Todos.Update updatedTodo |> ignore
                        do! dbContext.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore
                        return Some updatedTodo
                }

            member _.Delete id =
                async {
                    let! todoOpt = dbContext.Todos.FindAsync(id).AsTask() |> Async.AwaitTask

                    match Option.ofObj todoOpt with
                    | None -> return false
                    | Some todo ->
                        dbContext.Todos.Remove todo |> ignore
                        let! _ = dbContext.SaveChangesAsync() |> Async.AwaitTask
                        return true
                }
