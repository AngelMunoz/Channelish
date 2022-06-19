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
        join (fun ctx si ->
            task {
                ctx
                    .GetLogger()
                    .LogInformation("Connected! Socket Id: " + si.SocketId.ToString())

                return Ok
            })

        handle "status-change" (fun ctx si (msg: Message<{| message: string |}>) ->
            task {
                let logger = ctx.GetLogger()
                printfn "%A" msg.Payload
                logger.LogInformation("got message {message} from client with Socket Id: {socketId}", msg, si.SocketId)
                return ()
            })
    }

let app =
    application {
        use_router appRouter
        add_channel "/general" generalChannel
        use_gzip
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
