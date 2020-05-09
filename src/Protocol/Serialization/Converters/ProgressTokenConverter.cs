using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class ProgressTokenConverter : JsonConverter<ProgressToken>
    {
        public override void Write(Utf8JsonWriter writer, ProgressToken value, JsonSerializerOptions options)
        {
            if (value.IsLong)   JsonSerializer.Serialize(writer, value.Long, options);
            else if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
            else writer.WriteNull();
        }

        public override ProgressToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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


    }
}
