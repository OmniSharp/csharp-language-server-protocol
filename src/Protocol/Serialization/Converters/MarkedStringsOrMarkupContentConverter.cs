using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class MarkedStringsOrMarkupContentConverter : JsonConverter<MarkedStringsOrMarkupContent>
    {
        public override void Write(Utf8JsonWriter writer, MarkedStringsOrMarkupContent value, JsonSerializerOptions options)
        {
            if (value.HasMarkupContent)
            {
                  JsonSerializer.Serialize(writer, value.MarkupContent, options);
            }
            else
            {
                  JsonSerializer.Serialize(writer, value.MarkedStrings, options);
            }
        }

        public override MarkedStringsOrMarkupContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new MarkedStringsOrMarkupContent(result.ToObject<MarkupContent>(serializer));
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                var result = JArray.Load(reader);
                return new MarkedStringsOrMarkupContent(result.ToObject<Container<MarkedString>>(serializer));
            }
            if (reader.TokenType == JsonToken.String)
            {
                return new MarkedStringsOrMarkupContent(reader.Value as string);
            }

            return new MarkedStringsOrMarkupContent("");
        }


    }
}
