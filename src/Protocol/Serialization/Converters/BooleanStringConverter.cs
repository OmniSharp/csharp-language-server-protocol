using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanStringConverter : JsonConverter<BooleanString>
    {
        public override void WriteJson(JsonWriter writer, BooleanString value, JsonSerializer serializer)
        {
            var v = value as BooleanString?;
            if (v.HasValue)
            {
                new JValue(value.Value).WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
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
