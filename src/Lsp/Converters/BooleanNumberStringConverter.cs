using System;
using Lsp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lsp.Converters
{
    class BooleanNumberStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = value as BooleanNumberString?;
            if (v.HasValue)
            {
                new JValue(v.Value.Value).WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
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

        public override bool CanConvert(Type objectType) => objectType == typeof(BooleanNumberString);
    }
}