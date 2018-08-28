using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanNumberStringConverter : JsonConverter<BooleanNumberString>
    {
        public override void WriteJson(JsonWriter writer, BooleanNumberString value, JsonSerializer serializer)
        {
            new JValue(value.Value).WriteTo(writer);
        }

        public override BooleanNumberString ReadJson(JsonReader reader, Type objectType, BooleanNumberString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new BooleanNumberString((long)reader.Value);
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return new BooleanNumberString((string)reader.Value);
            }
            else if (reader.TokenType == JsonToken.Boolean)
            {
                return new BooleanNumberString((bool)reader.Value);
            }
            return new BooleanNumberString();
        }

        public override bool CanRead => true;
    }
}
