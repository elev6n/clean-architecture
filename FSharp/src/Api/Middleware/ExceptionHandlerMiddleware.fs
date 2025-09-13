namespace CleanArchitecture.Api.middleware

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System
open System.Net
open System.Collections.Generic
open System.Threading.Tasks

type ExceptionHandlerMiddleware(next: RequestDelegate, logger: ILogger<ExceptionHandlerMiddleware>) =
    member _.InvokeAsync(context: HttpContext) =
        async {
            try
                do! next.Invoke context |> Async.AwaitTask
            with
            | :? ArgumentException as ex ->
                logger.LogError(ex, "ArgumentException occured")
                context.Response.StatusCode <- int HttpStatusCode.BadRequest
                do! context.Response.WriteAsJsonAsync {| Error = ex.Message |} |> Async.AwaitTask

            | :? KeyNotFoundException as ex ->
                logger.LogError(ex, "Resource not found")
                context.Response.StatusCode <- int HttpStatusCode.NotFound

                do!
                    context.Response.WriteAsJsonAsync {| Error = "Resource not found" |}
                    |> Async.AwaitTask

            | ex ->
                logger.LogError(ex, "Unhandled exception occurred")
                context.Response.StatusCode <- int HttpStatusCode.InternalServerError

                do!
                    context.Response.WriteAsJsonAsync {| Error = "Internal server error" |}
                    |> Async.AwaitTask
        }
        |> Async.StartAsTask
        :> Task
