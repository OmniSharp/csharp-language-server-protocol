using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public class StringOrMarkupContent
    {
        public StringOrMarkupContent(string value)
        {
            String = value;
        }

        public StringOrMarkupContent(MarkupContent markupContent)
        {
            MarkupContent = markupContent;
        }

        public string String { get; }
        public bool HasString => this.MarkupContent == null;
        public MarkupContent MarkupContent { get; }
        public bool HasMarkupContent => String == null;

        public static implicit operator StringOrMarkupContent(string value)
        {
            return new StringOrMarkupContent(value);
        }

        public static implicit operator StringOrMarkupContent(MarkupContent markupContent)
        {
            return new StringOrMarkupContent(markupContent);
        }

        class Converter : JsonConverter<StringOrMarkupContent>
    {
        public override void Write(Utf8JsonWriter writer, StringOrMarkupContent value, JsonSerializerOptions options)
        {
            if (value.HasString)
            {
                writer.WriteStringValue(value.String);
            }
            else
            {
                JsonSerializer.Serialize(writer, value.MarkupContent, options);
            }
        }

        public override StringOrMarkupContent Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                string value = null;
                MarkupKind? kind = null;
                string propertyName = null;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); break;}
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        propertyName = reader.GetString();
                        continue;
                    }

                    switch (propertyName)
                    {
                        case nameof(value):
                            value = reader.GetString();
                            break;
                        case nameof(kind):
                            if (Enum.TryParse(reader.GetString(), out MarkupKind k))
                            {
                                kind = k;
                            }
                            break;
                        default:
                            throw new JsonException($"Unsupported property found {propertyName}");
                    }
                }

                return new StringOrMarkupContent(
                    new MarkupContent() {
                        Kind = kind?? MarkupKind.PlainText,
                        Value = value
                    }
                );
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new StringOrMarkupContent(reader.GetString());
            }

            return "";
        }
    }
    }
}
