using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class ProgressTokenConverter : JsonConverter<ProgressToken>
    {
        public override void WriteJson(JsonWriter writer, ProgressToken value, JsonSerializer serializer)
        {
            if (value.IsLong) serializer.Serialize(writer, value.Long);
            else if (value.IsString) serializer.Serialize(writer, value.String);
            else writer.WriteNull();
        }

        public override ProgressToken ReadJson(JsonReader reader, Type objectType, ProgressToken existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new ProgressToken((long)reader.Value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new ProgressToken((string)reader.Value);
            }

            return new ProgressToken(string.Empty);
        }

        public override bool CanRead => true;
    }

    // /// TODO: Add unit tests around this.
    // class ProgressParamsConverter : JsonConverter<ProgressParams>
    // {
    //     public override void WriteJson(JsonWriter writer, ProgressParams value, JsonSerializer serializer)
    //     {
    //         writer.WriteStartObject();
    //         writer.WritePropertyName(nameof(ProgressParams.Token));
    //         serializer.Serialize(writer, value.Token);
    //         writer.WritePropertyName(nameof(ProgressParams.Value));
    //         serializer.Serialize(writer, value.Value);
    //         writer.WriteEndObject();
    //     }

    //     public override ProgressParams ReadJson(JsonReader reader, Type objectType, ProgressParams existingValue, bool hasExistingValue, JsonSerializer serializer)
    //     {
    //         var instance = serializer.Deserialize

    //         if (objectType.IsGenericType) {
    //             var type = objectType.GetGenericArguments()[0];
    //             var instance = Activator.CreateInstance(objectType) as ProgressParams;

    //         } else {

    //         }

    //         if (reader.TokenType == JsonToken.Integer)
    //         {
    //             return new ProgressParams((long)reader.Value);
    //         }

    //         if (reader.TokenType == JsonToken.String)
    //         {
    //             return new ProgressParams((string)reader.Value);
    //         }

    //         return new ProgressParams();
    //     }

    //     public override bool CanRead => true;
    // }
}
