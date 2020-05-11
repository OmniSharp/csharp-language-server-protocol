using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(Converter))]
    public struct SymbolInformationOrDocumentSymbol
    {
        private DocumentSymbol _documentSymbol;
        private SymbolInformation _symbolInformation;

        public SymbolInformationOrDocumentSymbol(DocumentSymbol documentSymbol)
        {
            _documentSymbol = documentSymbol;
            _symbolInformation = default;
        }

        public SymbolInformationOrDocumentSymbol(SymbolInformation symbolInformation)
        {
            _documentSymbol = default;
            _symbolInformation = symbolInformation;
        }

        public bool IsDocumentSymbolInformation => _symbolInformation != null;
        public SymbolInformation SymbolInformation => _symbolInformation;

        public bool IsDocumentSymbol => _documentSymbol != null;
        public DocumentSymbol DocumentSymbol => _documentSymbol;

        public static SymbolInformationOrDocumentSymbol Create(SymbolInformation value)
        {
            return value;
        }

        public static SymbolInformationOrDocumentSymbol Create(DocumentSymbol value)
        {
            return value;
        }

        public static implicit operator SymbolInformationOrDocumentSymbol(SymbolInformation value)
        {
            return new SymbolInformationOrDocumentSymbol(value);
        }

        public static implicit operator SymbolInformationOrDocumentSymbol(DocumentSymbol value)
        {
            return new SymbolInformationOrDocumentSymbol(value);
        }

        class Converter : JsonConverter<SymbolInformationOrDocumentSymbol>
        {
            public override void Write(Utf8JsonWriter writer, SymbolInformationOrDocumentSymbol value,
                JsonSerializerOptions options)
            {
                if (value.IsDocumentSymbolInformation)
                {
                    JsonSerializer.Serialize(writer, value.SymbolInformation, options);
                }
                else if (value.IsDocumentSymbol)
                {
                    JsonSerializer.Serialize(writer, value.DocumentSymbol, options);
                }
                else
                {
                    writer.WriteNullValue();
                }
            }

            public override SymbolInformationOrDocumentSymbol Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                string name = null;
                SymbolKind kind = default;
                Container<SymbolTag> tags = null;
                bool deprecated = false;
                Location location = null;
                string containerName = null;
                string detail = null;
                Range range = null;
                Range selectionRange = null;
                Container<DocumentSymbol> children = null;
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
                        case nameof(name):
                            name = reader.GetString();
                            break;
                        case nameof(containerName):
                            containerName = reader.GetString();
                            break;
                        case nameof(detail):
                            detail = reader.GetString();
                            break;
                        case nameof(kind):
                            kind = JsonSerializer.Deserialize<SymbolKind>(ref reader, options);
                            break;
                        case nameof(deprecated):
                            deprecated = reader.GetBoolean();
                            break;
                        case nameof(location):
                            location = JsonSerializer.Deserialize<Location>(ref reader, options);
                            break;
                        case nameof(tags):
                            tags = JsonSerializer.Deserialize<Container<SymbolTag>>(ref reader, options);
                            break;
                        case nameof(children):
                            children = JsonSerializer.Deserialize<Container<DocumentSymbol>>(ref reader, options);
                            break;
                        case nameof(range):
                            range = JsonSerializer.Deserialize<Range>(ref reader, options);
                            break;
                        case nameof(selectionRange):
                            selectionRange = JsonSerializer.Deserialize<Range>(ref reader, options);
                            break;
                        default:
                            throw new JsonException($"Unsupported property found {propertyName}");
                    }
                }

                // SymbolInformation has property location, DocumentSymbol does not.
                if (location != null)
                {
                    return new SymbolInformation() {
                        Deprecated = deprecated,
                        Kind = kind,
                        Location = location,
                        Name = name,
                        Tags = tags,
                        ContainerName = containerName,
                    };
                }
                else
                {
                    return new DocumentSymbol() {
                        Children = children,
                        Deprecated = deprecated,
                        Detail = detail,
                        Kind = kind,
                        Name = name,
                        Range = range,
                        SelectionRange = selectionRange
                    };
                }
            }
        }
    }
}
