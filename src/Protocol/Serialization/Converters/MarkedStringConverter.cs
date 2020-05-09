using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class MarkedStringConverter : JsonConverter<MarkedString>
    {
        public override void Write(Utf8JsonWriter writer, MarkedString value, JsonSerializerOptions options)
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

        public override MarkedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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


    }
}
