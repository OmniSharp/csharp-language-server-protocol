using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(Converter))]
    public struct SemanticTokensOrSemanticTokensEdits
    {
        public SemanticTokensOrSemanticTokensEdits(SemanticTokensEdits semanticTokensEdits)
        {
            SemanticTokensEdits = semanticTokensEdits;
            SemanticTokens = null;
        }

        public SemanticTokensOrSemanticTokensEdits(SemanticTokens semanticTokens)
        {
            SemanticTokensEdits = null;
            SemanticTokens = semanticTokens;
        }

        public bool IsSemanticTokens => SemanticTokens != null;
        public SemanticTokens SemanticTokens { get; }

        public bool IsSemanticTokensEdits => SemanticTokensEdits != null;
        public SemanticTokensEdits SemanticTokensEdits { get; }

        public static implicit operator SemanticTokensOrSemanticTokensEdits(SemanticTokensEdits semanticTokensEdits)
        {
            return new SemanticTokensOrSemanticTokensEdits(semanticTokensEdits);
        }

        public static implicit operator SemanticTokensOrSemanticTokensEdits(SemanticTokens semanticTokens)
        {
            return new SemanticTokensOrSemanticTokensEdits(semanticTokens);
        }

        class Converter : JsonConverter<SemanticTokensOrSemanticTokensEdits>
    {
        public override void Write(Utf8JsonWriter writer, SemanticTokensOrSemanticTokensEdits value,
            JsonSerializerOptions options)
        {
            if (value.IsSemanticTokens)
            {
                JsonSerializer.Serialize(writer, value.SemanticTokens, options);
            }
            else if (value.IsSemanticTokensEdits)
            {
                JsonSerializer.Serialize(writer, value.SemanticTokensEdits, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }

        public override SemanticTokensOrSemanticTokensEdits Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            string resultId = null;
            var data = ImmutableArray<int>.Empty;
            Container<SemanticTokensEdit> edits = null;
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
                    case nameof(resultId):
                        resultId = reader.GetString();
                        break;
                    case nameof(data):
                        data = JsonSerializer.Deserialize<ImmutableArray<int>>(ref reader, options);
                        break;
                    case nameof(edits):
                        edits = JsonSerializer.Deserialize<Container<SemanticTokensEdit>>(ref reader, options);
                        break;
                    default:
                        throw new JsonException($"Unsupported property found {propertyName}");
                }
            }

            if (edits == null)
            {
                return new SemanticTokens() {
                    Data = data,
                    ResultId = resultId
                };
            }
            else
            {
                return new SemanticTokensEdits() {
                    Edits = edits,
                    ResultId = resultId
                };
            }
        }
    }
    }
}
