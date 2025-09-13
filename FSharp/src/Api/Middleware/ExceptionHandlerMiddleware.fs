namespace CleanArchitecture.Api.middleware

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Net
open System.Threading.Tasks
open CleanArchitecture.Domain.Exceptions

type ExceptionHandlerMiddleware(next: RequestDelegate, logger: ILogger<ExceptionHandlerMiddleware>) =
    member _.InvokeAsync(context: HttpContext) =
        async {
            try
                do! next.Invoke context |> Async.AwaitTask
            with
            | :? ValidationException as ex ->
                logger.LogWarning(ex, "Validation error")
                context.Response.StatusCode <- int HttpStatusCode.BadRequest
                do! context.Response.WriteAsJsonAsync({| Error = ex.Message |}) |> Async.AwaitTask
                
            | :? NotFoundException as ex ->
                logger.LogWarning(ex, "Resource not found")
                context.Response.StatusCode <- int HttpStatusCode.NotFound
                do! context.Response.WriteAsJsonAsync({| Error = ex.Message |}) |> Async.AwaitTask
                
            | :? ConflictException as ex ->
                logger.LogWarning(ex, "Conflict detected")
                context.Response.StatusCode <- int HttpStatusCode.Conflict
                do! context.Response.WriteAsJsonAsync({| Error = ex.Message |}) |> Async.AwaitTask
                
            | :? DomainException as ex ->
                logger.LogError(ex, "Domain error")
                context.Response.StatusCode <- int HttpStatusCode.BadRequest
                do! context.Response.WriteAsJsonAsync({| Error = ex.Message |}) |> Async.AwaitTask

            | ex ->
                logger.LogError(ex, "Unhandled exception occurred")
                context.Response.StatusCode <- int HttpStatusCode.InternalServerError

                do!
                    context.Response.WriteAsJsonAsync {| Error = "Internal server error" |}
                    |> Async.AwaitTask
        }
        |> Async.StartAsTask
        :> Task
