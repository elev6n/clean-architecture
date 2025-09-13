namespace CleanArchitecture.Api.Middleware

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Threading.Tasks
open System.Diagnostics

type RequestLoggingMiddleware(next: RequestDelegate, logger: ILogger<RequestLoggingMiddleware>) =
    member _.InvokeAsync(context: HttpContext) =
        async {
            let stopwatch = Stopwatch.StartNew()
            let request = context.Request

            logger.LogInformation("Starting request: {Method} {Path}", request.Method, request.Path)

            try
                do! next.Invoke context |> Async.AwaitTask
                stopwatch.Stop()

                logger.LogInformation(
                    "Completed request: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                )
            with ex ->
                stopwatch.Stop()

                logger.LogError(
                    ex,
                    "Request failed: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                )

            // reraise ()

        }
        |> Async.StartAsTask
        :> Task
