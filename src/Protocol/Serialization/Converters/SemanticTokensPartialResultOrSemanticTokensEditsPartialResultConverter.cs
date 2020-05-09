using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensPartialResultOrSemanticTokensEditsPartialResultConverter : JsonConverter<SemanticTokensPartialResultOrSemanticTokensEditsPartialResult>
    {
        public override void Write(Utf8JsonWriter writer, SemanticTokensPartialResultOrSemanticTokensEditsPartialResult value, JsonSerializerOptions options)
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
                writer.WriteNull();
            }
        }

        public override SemanticTokensPartialResultOrSemanticTokensEditsPartialResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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


    }
}
