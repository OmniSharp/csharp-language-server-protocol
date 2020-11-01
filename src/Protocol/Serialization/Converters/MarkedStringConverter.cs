using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class MarkedStringConverter : JsonConverter<MarkedString>
    {
        public override void WriteJson(JsonWriter writer, MarkedString value, JsonSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(value.Language))
            {
                writer.WriteValue(value.Value);
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("language");
                writer.WriteValue(value.Language);
                writer.WritePropertyName("value");
                writer.WriteValue(value.Value);
                writer.WriteEndObject();
            }
        }

        public override MarkedString ReadJson(JsonReader reader, Type objectType, MarkedString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new MarkedString(result["language"]?.Value<string>(), result["value"].Value<string>());
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new MarkedString((reader.Value as string)!);
            }

            return "";
        }

        public override bool CanRead => true;
    }
}
