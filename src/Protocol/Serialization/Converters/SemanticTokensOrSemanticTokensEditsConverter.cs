using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensOrSemanticTokensEditsConverter : JsonConverter<SemanticTokensOrSemanticTokensEdits>
    {
        public override void Write(Utf8JsonWriter writer, SemanticTokensOrSemanticTokensEdits value, JsonSerializerOptions options)
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
                writer.WriteNull();
            }
        }

        public override SemanticTokensOrSemanticTokensEdits Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("data"))
            {
                return new SemanticTokensOrSemanticTokensEdits(obj.ToObject<SemanticTokens>());
            }
            else
            {
                return new SemanticTokensOrSemanticTokensEdits(obj.ToObject<SemanticTokensEdits>());
            }
        }


    }
}
