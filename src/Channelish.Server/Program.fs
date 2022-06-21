open Giraffe
open Saturn
open Channelish.Server


let app =
    application {
        use_router Handlers.appRouter
        add_channel "/general" Channels.General
        use_json_serializer Helpers.JsonSerializer
        use_jwt_authentication Helpers.JwtSecret "http://localhost:5000"
        use_gzip
#if DEBUG
        url "http://0.0.0.0:5000"
        url "https://0.0.0.0:5001"

        use_cors "debug" (fun o ->
            o
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
            |> ignore)
#endif
    }

run app
