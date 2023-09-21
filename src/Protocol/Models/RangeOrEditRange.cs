using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

[JsonConverter(typeof(Converter))]
public partial record RangeOrEditRange
{
    public RangeOrEditRange(Range range)
    {
        Range = range;
        EditRange = null;
    }

    public RangeOrEditRange(EditRange editRange)
    {
        Range = null;
        EditRange = editRange;
    }

    public bool IsRange => Range is not null;
    public Range? Range { get; set; }

    public bool IsEditRange => EditRange != null;
    public EditRange? EditRange { get; }

    public static implicit operator RangeOrEditRange(Range range) => new RangeOrEditRange(range);

    public static implicit operator RangeOrEditRange(EditRange editRange) => new RangeOrEditRange(editRange);


    public class Converter : JsonConverter<RangeOrEditRange>
    {
        public override void WriteJson(JsonWriter writer, RangeOrEditRange value, JsonSerializer serializer)
        {
            if (value.IsRange)
                serializer.Serialize(writer, value.Range);
            if (value.IsEditRange)
                serializer.Serialize(writer, value.EditRange);
        }

        public override RangeOrEditRange ReadJson(
            JsonReader reader, Type objectType, RangeOrEditRange existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("insert"))
            {
                return new RangeOrEditRange(obj.ToObject<EditRange>(serializer));
            }

            return new RangeOrEditRange(obj.ToObject<Range>(serializer));
        }

        public override bool CanRead => true;
    }
}
