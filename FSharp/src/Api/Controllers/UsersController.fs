namespace CleanArchitecture.Api.Controllers

open System
open MediatR
open Microsoft.AspNetCore.Mvc
open CleanArchitecture.Domain.Common
open CleanArchitecture.Domain.User
open CleanArchitecture.Application.Users

[<ApiController>]
[<Route("api/[controller]")>]
type UsersController(mediator: IMediator) =
    inherit ControllerBase()

    [<HttpGet("{id:guid}")>]
    member _.GetUserById(id: Guid) =
        async {
            let entityId = EntityId id
            let! result = mediator.Send(GetUserByIdQuery entityId) |> Async.AwaitTask

            return
                match result with
                | Some user -> OkObjectResult user :> IActionResult
                | None -> NotFoundResult() :> IActionResult
        }
        |> Async.StartAsTask

    [<HttpGet>]
    member _.GetAllUsers() =
        async {
            let! users = mediator.Send(GetAllUsersQuery()) |> Async.AwaitTask
            return OkObjectResult users :> IActionResult
        }
        |> Async.StartAsTask

    [<HttpPost>]
    member _.CreateUser([<FromBody>] request: {| Email: string; Username: string |}) =
        async {
            let createRequest =
                { Email = Email request.Email
                  Username = Username request.Username }

            let! user = mediator.Send(CreateUserCommand createRequest) |> Async.AwaitTask
            return CreatedResult($"/api/users/{user.Id}", user) :> IActionResult
        }
        |> Async.StartAsTask
