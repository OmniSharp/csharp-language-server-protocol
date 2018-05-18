using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class BooleanStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = value as BooleanString?;
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

        public override bool CanConvert(Type objectType) => objectType == typeof(BooleanString);
    }
}