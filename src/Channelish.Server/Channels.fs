module Channelish.Server.Channels

open Microsoft.Extensions.Logging
open Giraffe

open Saturn
open Saturn.Channels

[<Literal>]
let GeneralChannel = "general"

let General =
    channel {
        join (fun ctx ci ->
            task {
                let logger = ctx.GetLogger($"{nameof GeneralChannel}:Join")


                if ctx.User.Identity.IsAuthenticated then
                    logger.LogInformation(
                        "Authenticated! Socket Id: "
                        + ci.SocketId.ToString()
                    )

                    let hub = ctx.GetService<ISocketHub>()

                    do!
                        hub.SendMessageToClients
                            $"/{GeneralChannel}"
                            "status-change"
                            {| Event = "Connected"
                               Client = ci.SocketId |}

                    return Ok
                else
                    logger.LogWarning(
                        "Not Authenticated! Socket Id: "
                        + ci.SocketId.ToString()
                    )

                    return Rejected "Not authenticated"
            })

        handle "status-change" (fun ctx ci (msg: Message<{| message: string |}>) ->
            task {
                let logger = ctx.GetLogger($"{nameof GeneralChannel}:StatusChange")
                let hub = ctx.GetService<ISocketHub>()
                logger.LogInformation("got message {message} from client with Socket Id: {socketId}", msg, ci.SocketId)
                do! hub.SendMessageToClients "/general" "status-change" {| Got = { msg with Ref = $"{ci.SocketId}" } |}
                return ()
            })

        terminate (fun ctx ci ->
            task {
                let logger = ctx.GetLogger($"{nameof GeneralChannel}:Terminate")
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
                let logger = ctx.GetLogger($"{nameof GeneralChannel}:ErrorHandler")
                logger.LogError("{SocketId} Error: {error}", ci.SocketId, err)
                return ()
            })

        not_found_handler (fun ctx ci msg ->
            task {
                let logger = ctx.GetLogger($"{nameof GeneralChannel}:NotFound")
                logger.LogInformation($"[{ci.SocketId} - Not Found] %A{msg}")
                return ()
            })
    }
