using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class RangeOrPlaceholderRangeConverter : JsonConverter<RangeOrPlaceholderRange>
    {
        public override void WriteJson(JsonWriter writer, RangeOrPlaceholderRange value, JsonSerializer serializer)
        {
            if (value.IsRange)
            {
                serializer.Serialize(writer, value.Range);
            }
            else
            {
                serializer.Serialize(writer, value.PlaceholderRange);
            }
        }

        public override RangeOrPlaceholderRange ReadJson(JsonReader reader, Type objectType, RangeOrPlaceholderRange existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new RangeOrPlaceholderRange((Range) null);
        }

        public override bool CanRead => false;
    }
}
