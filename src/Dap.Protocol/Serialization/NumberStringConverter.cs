using System;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    class NumberStringConverter : JsonConverter<NumberString>
    {
        public override void Write(Utf8JsonWriter writer, NumberString value, JsonSerializerOptions options)
        {
            if (value.IsLong)   JsonSerializer.Serialize(writer, value.Long, options);
            else if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
            else writer.WriteNull();
        }

        public override NumberString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new NumberString((long)reader.Value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new NumberString((string)reader.Value);
            }

            return new NumberString();
        }


    }
}
