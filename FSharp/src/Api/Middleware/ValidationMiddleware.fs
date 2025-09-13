namespace CleanArchitecture.Api.Middleware

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Threading.Tasks

type ValidationMiddleware(next: RequestDelegate, logger: ILogger<ValidationMiddleware>) =
    member _.InvokeAsync(context: HttpContext) =
        async {
            if
                context.Request.Method = HttpMethods.Post
                || context.Request.Method = HttpMethods.Put
            then

                if not (context.Request.Headers.ContainsKey "Content-Type") then
                    context.Response.StatusCode <- 415

                    do!
                        context.Response.WriteAsJsonAsync {| Error = "Content-Type header is required" |}
                        |> Async.AwaitTask

                    return ()
                else
                    do! next.Invoke context |> Async.AwaitTask
            else
                do! next.Invoke context |> Async.AwaitTask
        }
        |> Async.StartAsTask
        :> Task
