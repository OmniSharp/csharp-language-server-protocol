using System;
using Newtonsoft.Json;
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
}
