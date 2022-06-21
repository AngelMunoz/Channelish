module Channelish.Server.Handlers

open Giraffe
open Saturn


module private Auth =
    let Signup next ctx =
        task { return! (text "Signup") next ctx }

    let Signin next ctx =
        task { return! (text "Signup") next ctx }


let private authenticatedApiPipeline =
    pipeline {
        must_accept [ "application/json" ]
        requires_authentication (jsonChunked {| error = "Not authorized" |})
    }

let appRouter =
    router {
        get "/" (text "Hello World!")

        forward
            "/api"
            (router {
                forward
                    "/auth"
                    (router {
                        get "/signin" Auth.Signin

                        get "/signup" Auth.Signup
                    })

                forward "/" (router { pipe_through authenticatedApiPipeline })
            })
    }
