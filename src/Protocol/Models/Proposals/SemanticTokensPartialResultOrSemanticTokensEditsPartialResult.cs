using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(Converter))]
    public struct SemanticTokensPartialResultOrSemanticTokensEditsPartialResult
    {
        public SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensEditsPartialResult semanticTokensEditsPartialResult)
        {
            SemanticTokensEditsPartialResult = semanticTokensEditsPartialResult;
            SemanticTokensPartialResult = null;
        }

        public SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensPartialResult semanticTokensPartialResult)
        {
            SemanticTokensEditsPartialResult = null;
            SemanticTokensPartialResult = semanticTokensPartialResult;
        }

        public bool IsSemanticTokensPartialResult => SemanticTokensPartialResult != null;
        public SemanticTokensPartialResult SemanticTokensPartialResult { get; }

        public bool IsSemanticTokensEditsPartialResult => SemanticTokensEditsPartialResult != null;
        public SemanticTokensEditsPartialResult SemanticTokensEditsPartialResult { get; }

        public static implicit operator SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensEditsPartialResult semanticTokensEditsPartialResult)
        {
            return new SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(semanticTokensEditsPartialResult);
        }

        public static implicit operator SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensPartialResult semanticTokensPartialResult)
        {
            return new SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(semanticTokensPartialResult);
        }

        class Converter : JsonConverter<
            SemanticTokensPartialResultOrSemanticTokensEditsPartialResult>
        {
            public override void Write(Utf8JsonWriter writer,
                SemanticTokensPartialResultOrSemanticTokensEditsPartialResult value, JsonSerializerOptions options)
            {
                if (value.IsSemanticTokensPartialResult)
                {
                    JsonSerializer.Serialize(writer, value.SemanticTokensPartialResult, options);
                }
                else if (value.IsSemanticTokensEditsPartialResult)
                {
                    JsonSerializer.Serialize(writer, value.SemanticTokensEditsPartialResult, options);
                }
                else
                {
                    writer.WriteNullValue();
                }
            }

            public override SemanticTokensPartialResultOrSemanticTokensEditsPartialResult Read(
                ref Utf8JsonReader reader,
                Type typeToConvert, JsonSerializerOptions options)
            {
                var data = ImmutableArray<int>.Empty;
                Container<SemanticTokensEdit> edits = null;
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

                if (edits != null)
                {
                    return new SemanticTokensEditsPartialResult() {
                        Edits = edits
                    };
                }
                else
                {
                    return new SemanticTokensPartialResult() {
                        Data = data
                    };
                }
            }
        }
    }
}
