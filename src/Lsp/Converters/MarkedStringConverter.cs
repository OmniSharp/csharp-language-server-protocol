using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Converters
{
    class MarkedStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = value as MarkedString;
            if (string.IsNullOrWhiteSpace(v.Language))
            {
                writer.WriteValue(v.Value);
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("language");
                writer.WriteValue(v.Language);
                writer.WritePropertyName("value");
                writer.WriteValue(v.Value);
                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new MarkedString(result["language"]?.Value<string>(), result["value"]?.Value<string>());
            }
            if (reader.TokenType == JsonToken.String)
            {
                return new MarkedString(reader.Value as string);
            }

            return "";
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(MarkedString);
    }
}