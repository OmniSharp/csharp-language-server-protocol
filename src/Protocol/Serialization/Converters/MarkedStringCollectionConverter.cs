using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class MarkedStringCollectionConverter : JsonConverter<Container<MarkedString>>
    {
        public override void WriteJson(JsonWriter writer, Container<MarkedString> value, JsonSerializer serializer)
        {
            var v = value.ToArray();
            if (v.Length == 1)
            {
                serializer.Serialize(writer, v[0]);
                return;
            }

            serializer.Serialize(writer, v);
        }

        public override Container<MarkedString> ReadJson(
            JsonReader reader, Type objectType, Container<MarkedString> existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new Container<MarkedString>(JArray.Load(reader).ToObject<IEnumerable<MarkedString>>(serializer));
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                return new Container<MarkedString>(JObject.Load(reader).ToObject<MarkedString>(serializer));
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new Container<MarkedString>((reader.Value as string)!);
            }

            return new Container<MarkedString>();
        }

        public override bool CanRead => true;
    }
}
