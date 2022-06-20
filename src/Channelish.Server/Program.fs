open Giraffe
open Saturn
open Saturn.Channels
open Microsoft.Extensions.Logging

let apiRouter =

    let authenticatedApiPipeline =
        pipeline {
            must_accept [ "application/json" ]
            requires_authentication (jsonChunked {| error = "Not authorized" |})
        }

    let authRouter =
        router {
            get "/signin" (text "Hello login")
            get "/signup" (text "Hello signup")
        }

    let protectedRouter = router { pipe_through authenticatedApiPipeline }

    router {
        forward "/auth" authRouter
        forward "/" protectedRouter
    }

let appRouter =
    router {
        get "/" (text "Hello World!")
        forward "/api" apiRouter
    }

let generalChannel =
    channel {
        join (fun ctx ci ->
            task {
                ctx
                    .GetLogger("General:Join")
                    .LogInformation("Connected! Socket Id: " + ci.SocketId.ToString())

                let hub = ctx.GetService<ISocketHub>()

                do!
                    hub.SendMessageToClients
                        "/general"
                        "status-change"
                        {| Event = "Connected"
                           Client = ci.SocketId |}

                return Ok
            })

        handle "status-change" (fun ctx ci (msg: Message<{| message: string |}>) ->
            task {
                let logger = ctx.GetLogger("General:StatusChange")
                let hub = ctx.GetService<ISocketHub>()
                logger.LogInformation("got message {message} from client with Socket Id: {socketId}", msg, ci.SocketId)
                do! hub.SendMessageToClients "/general" "status-change" {| Got = { msg with Ref = $"{ci.SocketId}" } |}
                return ()
            })

        terminate (fun ctx ci ->
            task {
                let logger = ctx.GetLogger("General:Terminate")
                let hub = ctx.GetService<ISocketHub>()
                logger.LogInformation($"{ci.SocketId} - Terminated")

                do!
                    hub.SendMessageToClients
                        "/general"
                        "status-change"
                        {| Event = "Left"
                           Client = ci.SocketId |}

                return ()
            })

        error_handler (fun ctx ci msg err ->
            task {
                let logger = ctx.GetLogger("General:ErrorHandler")
                logger.LogError("{SocketId} Error: {error}", ci.SocketId, err)
                return ()
            })

        not_found_handler (fun ctx ci msg ->
            task {
                let logger = ctx.GetLogger("General:NotFound")
                logger.LogInformation($"[{ci.SocketId} - Not Found] %A{msg}")
                return ()
            })
    }

let app =
    application {
        use_router appRouter
        add_channel "/general" generalChannel
        use_gzip
        url "http://0.0.0.0:5000"
        url "https://0.0.0.0:5001"
#if DEBUG
        use_cors "debug" (fun o ->
            o
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
            |> ignore)
#endif
    }

run app
