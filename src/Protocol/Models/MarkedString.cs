using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// MarkedString can be used to render human readable text. It is either a markdown string
    /// or a code-block that provides a language and a code snippet. The language identifier
    /// is sematically equal to the optional language identifier in fenced code blocks in GitHub
    /// issues. See https://help.github.com/articles/creating-and-highlighting-code-blocks/#syntax-highlighting
    ///
    /// The pair of a language and a value is an equivalent to markdown:
    /// ```${language}
    /// ${value}
    /// ```
    ///
    /// Note that markdown strings will be sanitized - that means html will be escaped.
    /// </summary>
    [JsonConverter(typeof(Converter))]
    public class MarkedString
    {
        public MarkedString(string value)
        {
            Value = value;
        }

        public MarkedString(string language, string value) : this(value)
        {
            Language = language;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Language { get; }

        public string Value { get; }

        public static implicit operator MarkedString(string value)
        {
            return new MarkedString(value);
        }

        class Converter : JsonConverter<MarkedString>
        {
            public override void Write(Utf8JsonWriter writer, MarkedString value, JsonSerializerOptions options)
            {
                if (string.IsNullOrWhiteSpace(value.Language))
                {
                    writer.WriteStringValue(value.Value);
                }
                else
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("language");
                    writer.WriteStringValue(value.Language);
                    writer.WritePropertyName("value");
                    writer.WriteStringValue(value.Value);
                    writer.WriteEndObject();
                }
            }

            public override MarkedString Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    string language = null;
                    string value = null;
                    string propertyName = null;
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            reader.Read();
                            break;
                        }

                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            propertyName = reader.GetString();
                            continue;
                        }

                        switch (propertyName)
                        {
                            case nameof(language):
                                language = reader.GetString();
                                break;
                            case nameof(value):
                                value = reader.GetString();
                                break;
                            default:
                                throw new JsonException($"Unsupported property found {propertyName}");
                        }
                    }

                    return new MarkedString(language, value);
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    return new MarkedString(reader.GetString());
                }

                return "";
            }
        }
    }
}
