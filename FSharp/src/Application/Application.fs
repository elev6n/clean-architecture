namespace CleanArchitecture.Application

open MediatR
open CleanArchitecture.Domain.Common
open CleanArchitecture.Domain.RepositoryInterfaces

module Users =
    open CleanArchitecture.Domain.User

    type GetUserByIdQuery(id: EntityId) =
        interface IRequest<User option>
        member _.Id = id

    type GetUserByEmailQuery(email: Email) =
        interface IRequest<User option>
        member _.Email = email

    type GetAllUsersQuery() =
        interface IRequest<User list>

    type CreateUserCommand(request: CreateUserRequest) =
        interface IRequest<User>
        member _.Request = request

    type UserQueryHandler(repository: IUserRepository) =
        interface IRequestHandler<GetUserByIdQuery, User option> with
            member _.Handle(request, _) =
                async { return! repository.GetById request.Id } |> Async.StartAsTask

        interface IRequestHandler<GetUserByEmailQuery, User option> with
            member _.Handle(request, _) =
                async { return! repository.GetByEmail request.Email } |> Async.StartAsTask

        interface IRequestHandler<GetAllUsersQuery, User list> with
            member _.Handle(_, _) =
                async { return! repository.GetAll() } |> Async.StartAsTask

    type CreateUserCommandHandler(repository: IUserRepository) =
        interface IRequestHandler<CreateUserCommand, User> with
            member _.Handle(request, _) =
                async { return! repository.Create request.Request } |> Async.StartAsTask

module Todos =
    open CleanArchitecture.Domain.Todo

    type GetTodoByIdQuery(id: EntityId) =
        interface IRequest<Todo option>
        member _.Id = id

    type GetUserTodosQuery(userId: EntityId) =
        interface IRequest<Todo list>
        member _.UserId = userId

    type CreateTodoCommand(request: CreateTodoRequest) =
        interface IRequest<Todo>
        member _.Request = request

    type UpdateTodoCommand(request: UpdateTodoRequest) =
        interface IRequest<Todo option>
        member _.Request = request

    type DeleteTodoCommand(id: EntityId) =
        interface IRequest<bool>
        member _.Id = id

    type TodoQueryHandler(repository: ITodoRepository) =
        interface IRequestHandler<GetTodoByIdQuery, Todo option> with
            member _.Handle(request, _) =
                async { return! repository.GetById request.Id } |> Async.StartAsTask

        interface IRequestHandler<GetUserTodosQuery, Todo list> with
            member _.Handle(request, _) =
                async {
                    return! repository.GetByUserId request.UserId // Исправлено на GetByUserId
                }
                |> Async.StartAsTask

    type TodoCommandHandler(repository: ITodoRepository) =
        interface IRequestHandler<CreateTodoCommand, Todo> with
            member _.Handle(request, _) =
                async { return! repository.Create request.Request } |> Async.StartAsTask

        interface IRequestHandler<UpdateTodoCommand, Todo option> with
            member _.Handle(request, _) =
                async { return! repository.Update request.Request } |> Async.StartAsTask

        interface IRequestHandler<DeleteTodoCommand, bool> with
            member _.Handle(request, _) =
                async { return! repository.Delete request.Id } |> Async.StartAsTask
