using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanStringConverter : JsonConverter<BooleanString>
    {
        public override void WriteJson(JsonWriter writer, BooleanString value, JsonSerializer serializer)
        {
            if (value.IsBool) serializer.Serialize(writer, value.Bool);
            if (value.IsString) serializer.Serialize(writer, value.String);
        }

        public override BooleanString ReadJson(JsonReader reader, Type objectType, BooleanString existingValue, bool hasExistingValue, JsonSerializer serializer)
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

        public override bool CanRead => true;
    }
}
