using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanNumberStringConverter : JsonConverter<BooleanNumberString>
    {
        public override void Write(Utf8JsonWriter writer, BooleanNumberString value, JsonSerializerOptions options)
        {
            if (value.IsBool)   JsonSerializer.Serialize(writer, value.Bool, options);
            else if (value.IsLong)   JsonSerializer.Serialize(writer, value.Long, options);
            else if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
            else writer.WriteNull();
        }

        public override BooleanNumberString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new BooleanNumberString((long)reader.Value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new BooleanNumberString((string)reader.Value);
            }

            if (reader.TokenType == JsonToken.Boolean)
            {
                return new BooleanNumberString((bool)reader.Value);
            }

            return new BooleanNumberString();
        }


    }
}
