using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanStringConverter : JsonConverter<BooleanString>
    {
        public override void Write(Utf8JsonWriter writer, BooleanString value, JsonSerializerOptions options)
        {
            if (value.IsBool)   JsonSerializer.Serialize(writer, value.Bool, options);
            if (value.IsString)   JsonSerializer.Serialize(writer, value.String, options);
        }

        public override BooleanString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return new BooleanString((string)reader.Value);
                case JsonToken.Boolean:
                    return new BooleanString((bool)reader.Value);
            }

            return new BooleanString();
        }


    }
}
