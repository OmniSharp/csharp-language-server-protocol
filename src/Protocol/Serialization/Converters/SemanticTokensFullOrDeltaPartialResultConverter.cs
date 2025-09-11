using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public class SemanticTokensFullOrDeltaPartialResultConverter : JsonConverter<SemanticTokensFullOrDeltaPartialResult>
    {
        public override void WriteJson(JsonWriter writer, SemanticTokensFullOrDeltaPartialResult value, JsonSerializer serializer)
        {
            if (value.IsDelta)
            {
                serializer.Serialize(writer, value.Delta);
            }
            else if (value.IsFull)
            {
                serializer.Serialize(writer, value.Full);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override SemanticTokensFullOrDeltaPartialResult ReadJson(
            JsonReader reader, Type objectType, SemanticTokensFullOrDeltaPartialResult existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("data"))
            {
                return new SemanticTokensFullOrDeltaPartialResult(obj.ToObject<SemanticTokensPartialResult>(serializer));
            }

            return new SemanticTokensFullOrDeltaPartialResult(obj.ToObject<SemanticTokensDeltaPartialResult>(serializer));
        }

        public override bool CanRead => true;
    }
}
