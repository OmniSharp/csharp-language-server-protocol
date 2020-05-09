using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class StringOrMarkupContentConverter : JsonConverter<StringOrMarkupContent>
    {
        public override void Write(Utf8JsonWriter writer, StringOrMarkupContent value, JsonSerializerOptions options)
        {
            if (value.HasString)
            {
                writer.WriteValue(value.String);
            }
            else
            {
                  JsonSerializer.Serialize(writer, value.MarkupContent, options);
            }
        }

        public override StringOrMarkupContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new StringOrMarkupContent(
                    new MarkupContent() {
                        Kind = Enum.TryParse<MarkupKind>(result["kind"]?.Value<string>(), true, out var kind) ? kind : MarkupKind.PlainText,
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


    }
}
