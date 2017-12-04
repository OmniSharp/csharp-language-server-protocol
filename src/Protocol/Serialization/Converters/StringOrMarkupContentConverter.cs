using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Converters
{
    public class StringOrMarkupContentConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = value as StringOrMarkupContent;
            if (v.HasString)
            {
                writer.WriteValue(v.String);
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("kind");
                writer.WriteValue(v.MarkupContent.Kind);
                writer.WritePropertyName("value");
                writer.WriteValue(v.MarkupContent.Value);
                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new StringOrMarkupContent(
                    new MarkupContent() {
                        Kind = result["kind"]?.Value<MarkupKind>() ?? MarkupKind.Plaintext,
                        Value = result["value"]?.Value<string>()
                    }
                );
            }
            if (reader.TokenType == JsonToken.String)
            {
                return new StringOrMarkupContent(reader.Value as string);
            }

            return "";
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(StringOrMarkupContent);
    }
}
