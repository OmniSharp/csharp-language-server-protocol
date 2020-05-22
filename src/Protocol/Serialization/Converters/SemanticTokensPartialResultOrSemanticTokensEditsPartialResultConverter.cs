using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensPartialResultOrSemanticTokensEditsPartialResultConverter : JsonConverter<SemanticTokensPartialResultOrSemanticTokensEditsPartialResult>
    {
        public override void WriteJson(JsonWriter writer, SemanticTokensPartialResultOrSemanticTokensEditsPartialResult value, JsonSerializer serializer)
        {
            if (value.IsSemanticTokensPartialResult)
            {
                serializer.Serialize(writer, value.SemanticTokensPartialResult);
            }
            else if (value.IsSemanticTokensEditsPartialResult)
            {
                serializer.Serialize(writer, value.SemanticTokensEditsPartialResult);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override SemanticTokensPartialResultOrSemanticTokensEditsPartialResult ReadJson(JsonReader reader, Type objectType, SemanticTokensPartialResultOrSemanticTokensEditsPartialResult existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("start"))
            {
                return new SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(obj.ToObject<SemanticTokensEditsPartialResult>());
            }
            else
            {
                return new SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(obj.ToObject<SemanticTokensPartialResult>());
            }
        }

        public override bool CanRead => true;
    }
}
