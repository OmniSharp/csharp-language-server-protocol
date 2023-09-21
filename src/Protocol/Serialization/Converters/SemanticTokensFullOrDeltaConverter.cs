using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class SemanticTokensFullOrDeltaConverter : JsonConverter<SemanticTokensFullOrDelta>
    {
        public override void WriteJson(JsonWriter writer, SemanticTokensFullOrDelta value, JsonSerializer serializer)
        {
            if (value.IsFull)
            {
                serializer.Serialize(writer, value.Full);
            }
            else if (value.IsDelta)
            {
                serializer.Serialize(writer, value.Delta);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override SemanticTokensFullOrDelta ReadJson(
            JsonReader reader, Type objectType, SemanticTokensFullOrDelta existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("data"))
            {
                return new SemanticTokensFullOrDelta(obj.ToObject<SemanticTokens>(serializer));
            }

            return new SemanticTokensFullOrDelta(obj.ToObject<SemanticTokensDelta>(serializer));
        }

        public override bool CanRead => true;
    }
}
