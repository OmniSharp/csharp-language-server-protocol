using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal class RangeOrPlaceholderRangeConverter : JsonConverter<RangeOrPlaceholderRange?>
    {
        public override void WriteJson(JsonWriter writer, RangeOrPlaceholderRange? value, JsonSerializer serializer)
        {
            if (value?.IsRange == true)
            {
                serializer.Serialize(writer, value.Range);
            }
            else if (value?.IsPlaceholderRange == true)
            {
                serializer.Serialize(writer, value.PlaceholderRange);
            }
            else if (value?.IsDefaultBehavior == true)
            {
                serializer.Serialize(writer, value.DefaultBehavior);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override RangeOrPlaceholderRange? ReadJson(
            JsonReader reader, Type objectType, RangeOrPlaceholderRange? existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            if (reader.TokenType is JsonToken.StartObject)
            {
                var obj = (JToken.ReadFrom(reader) as JObject)!;
                return obj.ContainsKey("placeholder")
                    ? new RangeOrPlaceholderRange(obj.ToObject<PlaceholderRange>(serializer))
                    : obj.ContainsKey("defaultBehavior")
                        ? new RangeOrPlaceholderRange(
                            obj.ToObject<RenameDefaultBehavior>(serializer)
                        )
                        : new RangeOrPlaceholderRange(
                            obj.ToObject<Range>(serializer)
                        );
            }

            return null;
        }
    }
}
