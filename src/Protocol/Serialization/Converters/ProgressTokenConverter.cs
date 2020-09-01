using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class ProgressTokenConverter : JsonConverter<ProgressToken>
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
                return new ProgressToken((long) reader.Value);
            }

            if (reader.TokenType == JsonToken.String && reader.Value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return new ProgressToken(str);
            }

            return null;
        }

        public override bool CanRead => true;
    }
}
