module Tests

open System
open Xunit
open CleanArchitecture.Domain
open CleanArchitecture.Domain.Common
open CleanArchitecture.Application.Users
open CleanArchitecture.Application.Todos

module UserTests =
    open User

    [<Fact>]
    let ``CreateUserCommand should create valid user`` () =
        let request: CreateUserRequest =
            { Email = Email "test@example.com"
              Username = Username "testuser" }

        let command = CreateUserCommand request
        Assert.Equal(request, command.Request)

    [<Fact>]
    let ``GetUserByIdQuery should have correct id`` () =
        let id = EntityId(Guid.NewGuid())
        let query = GetUserByIdQuery id
        Assert.Equal(id, query.Id)

module TodoTests =
    open Todo

    [<Fact>]
    let ``CreateTodoComman should create valid todo`` () =
        let userId = EntityId(Guid.NewGuid())

        let request: CreateTodoRequest =
            { Title = "Test Todo"
              Description = Some "Test description"
              UserId = userId }

        let command = CreateTodoCommand request
        Assert.Equal(request, command.Request)

    [<Fact>]
    let ``GetUserTodosQuery should have correct user id`` () =
        let userId = EntityId(Guid.NewGuid())
        let query = GetUserTodosQuery userId
        Assert.Equal(userId, query.UserId)
