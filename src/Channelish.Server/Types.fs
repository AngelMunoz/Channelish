namespace Channelish.Server

open System
open MongoDB.Bson
open MongoDB.Bson.Serialization.IdGenerators
open MongoDB.Bson.Serialization.Attributes

type User =
    { [<BsonId(IdGenerator = typeof<GuidGenerator>)>]
      id: Guid
      name: string
      email: string }

[<Struct>]
type SignupPayload =
    { name: string
      email: string
      password: string }

[<Struct>]
type LoginPayload = { email: string; password: string }
