using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class GlobPatternConverter : JsonConverter<GlobPattern>
    {
        public override void WriteJson(JsonWriter writer, GlobPattern value, JsonSerializer serializer)
        {
            if (value.HasPattern)
            {
                writer.WriteValue(value.Pattern);
            }
            else
            {
                serializer.Serialize(writer, value.RelativePattern);
            }
        }

        public override GlobPattern ReadJson(
            JsonReader reader, Type objectType, GlobPattern existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new GlobPattern(( reader.Value as string )!);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                return new GlobPattern(JObject.Load(reader).ToObject<RelativePattern>(serializer));
            }

            return new GlobPattern("");
        }

        public override bool CanRead => true;
    }
}
