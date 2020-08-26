using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(RangeOrPlaceholderRangeConverter))]
    public class RangeOrPlaceholderRange
    {
        private Range? _range;
        private PlaceholderRange? _placeholderRange;

        public RangeOrPlaceholderRange(Range value)
        {
            _range = value;
            _placeholderRange = default;
        }

        public RangeOrPlaceholderRange(PlaceholderRange value)
        {
            _range = default;
            _placeholderRange = value;
        }

        public bool IsPlaceholderRange => _placeholderRange != null;

        public PlaceholderRange? PlaceholderRange
        {
            get => _placeholderRange;
            set {
                _placeholderRange = value;
                _range = null;
            }
        }

        public bool IsRange => _range is not null;

        public Range? Range
        {
            get => _range;
            set {
                _placeholderRange = default;
                _range = value;
            }
        }

        public object? RawValue
        {
            get {
                if (IsPlaceholderRange) return PlaceholderRange;
                if (IsRange) return Range;
                return default;
            }
        }

        public static implicit operator RangeOrPlaceholderRange(PlaceholderRange value) => new RangeOrPlaceholderRange(value);

        public static implicit operator RangeOrPlaceholderRange(Range value) => new RangeOrPlaceholderRange(value);
    }
}
