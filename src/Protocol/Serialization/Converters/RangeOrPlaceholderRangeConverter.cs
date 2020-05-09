using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    class RangeOrPlaceholderRangeConverter : JsonConverter<RangeOrPlaceholderRange>
    {
        public override void Write(Utf8JsonWriter writer, RangeOrPlaceholderRange value, JsonSerializerOptions options)
        {
            if (value.IsRange)
            {
                  JsonSerializer.Serialize(writer, value.Range, options);
            }
            else
            {
                  JsonSerializer.Serialize(writer, value.PlaceholderRange, options);
            }
        }

        public override RangeOrPlaceholderRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new RangeOrPlaceholderRange((Range) null);
        }


    }
}
