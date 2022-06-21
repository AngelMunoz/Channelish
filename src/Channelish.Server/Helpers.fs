module Helpers

open System.Text.Json
open System.Text.Json.Serialization
open MongoDB.Bson
open Giraffe
open System
open System.Threading.Tasks

type ObjectIdConverter() =
    inherit JsonConverter<ObjectId>()

    override _.Read(reader: byref<Utf8JsonReader>, typeToConvert: Type, options: JsonSerializerOptions) =
        ObjectId.Parse(reader.GetString())

    override _.Write(writer: Utf8JsonWriter, value: ObjectId, options: JsonSerializerOptions) =
        writer.WriteStringValue(value.ToString())

let JwtSecret =
    System.Environment.GetEnvironmentVariable("CHANNELISH_JWT_SECRET")
    |> Option.ofObj
    |> Option.defaultValue "wow much secret :9"

let JsonSerializer =
    let opts =
        JsonSerializerOptions(
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        )

    opts.Converters.Add(JsonFSharpConverter())
    opts.Converters.Add(ObjectIdConverter())

    { new Json.ISerializer with
        member __.Deserialize<'T>(arg1: byte[]) : 'T =
            let spn = ReadOnlySpan(arg1)
            JsonSerializer.Deserialize<'T>(spn, opts)

        member __.Deserialize<'T>(arg1: string) : 'T =
            JsonSerializer.Deserialize<'T>(arg1, opts)

        member __.DeserializeAsync(arg1: IO.Stream) : Task<'T> =
            JsonSerializer
                .DeserializeAsync<'T>(arg1, opts)
                .AsTask()

        member __.SerializeToBytes<'T>(arg1: 'T) : byte array =
            JsonSerializer.SerializeToUtf8Bytes(arg1, opts)

        member __.SerializeToStreamAsync<'T> (arg1: 'T) (arg2: IO.Stream) : Task =
            JsonSerializer.SerializeAsync(arg2, arg1, opts)

        member __.SerializeToString<'T>(arg1: 'T) : string =
            JsonSerializer.Serialize(arg1, typeof<'T>, opts) }
