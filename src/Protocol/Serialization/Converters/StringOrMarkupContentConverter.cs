using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class StringOrMarkupContentConverter : JsonConverter
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
                serializer.Serialize(writer, v.MarkupContent);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new StringOrMarkupContent(
                    new MarkupContent() {
                        Kind = Enum.TryParse<MarkupKind>(result["kind"]?.Value<string>(), true, out var kind) ? kind : MarkupKind.Plaintext,
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
