open Giraffe
open Saturn
open Saturn.Channels
open Microsoft.Extensions.Logging

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
        use_router (text "Hello World from Saturn")
        add_channel "/general" generalChannel
    }

run app
