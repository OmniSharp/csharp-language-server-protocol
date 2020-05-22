using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensOrSemanticTokensEditsConverter : JsonConverter<SemanticTokensOrSemanticTokensEdits>
    {
        public override void WriteJson(JsonWriter writer, SemanticTokensOrSemanticTokensEdits value, JsonSerializer serializer)
        {
            if (value.IsSemanticTokens)
            {
                serializer.Serialize(writer, value.SemanticTokens);
            }
            else if (value.IsSemanticTokensEdits)
            {
                serializer.Serialize(writer, value.SemanticTokensEdits);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override SemanticTokensOrSemanticTokensEdits ReadJson(JsonReader reader, Type objectType, SemanticTokensOrSemanticTokensEdits existingValue, bool hasExistingValue, JsonSerializer serializer)
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

        public override bool CanRead => true;
    }
}
