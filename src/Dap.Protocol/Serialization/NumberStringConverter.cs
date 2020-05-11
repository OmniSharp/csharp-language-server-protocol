using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    class NumberStringConverter : JsonConverter<NumberString>
    {
        public override void Write(Utf8JsonWriter writer, NumberString value, JsonSerializerOptions options)
        {
            if (value.IsLong) JsonSerializer.Serialize(writer, value.Long, options);
            else if (value.IsString) JsonSerializer.Serialize(writer, value.String, options);
            else writer.WriteNullValue();
        }

        public override NumberString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return new NumberString(reader.GetInt64());
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new NumberString(reader.GetString());
            }

            return new NumberString();
        }
    }
}
