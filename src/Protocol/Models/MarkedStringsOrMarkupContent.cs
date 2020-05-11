using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public class MarkedStringsOrMarkupContent
    {
        public MarkedStringsOrMarkupContent(params MarkedString[] markedStrings)
        {
            MarkedStrings = markedStrings;
        }

        public MarkedStringsOrMarkupContent(IEnumerable<MarkedString> markedStrings)
        {
            MarkedStrings = markedStrings.ToArray();
        }

        public MarkedStringsOrMarkupContent(MarkupContent markupContent)
        {
            MarkupContent = markupContent;
        }

        public Container<MarkedString> MarkedStrings { get; }
        public bool HasMarkedStrings => MarkupContent == null;
        public MarkupContent MarkupContent { get; }
        public bool HasMarkupContent => MarkedStrings == null;

        class Converter : JsonConverter<MarkedStringsOrMarkupContent>
        {
            public override void Write(Utf8JsonWriter writer, MarkedStringsOrMarkupContent value,
                JsonSerializerOptions options)
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

            public override MarkedStringsOrMarkupContent Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    return new MarkedStringsOrMarkupContent(JsonSerializer.Deserialize<MarkupContent>(ref reader, options));
                }

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    return new MarkedStringsOrMarkupContent(
                        JsonSerializer.Deserialize<Container<MarkedString>>(ref reader, options));
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    return new MarkedStringsOrMarkupContent(reader.GetString());
                }

                return new MarkedStringsOrMarkupContent("");
            }
        }
    }
}
