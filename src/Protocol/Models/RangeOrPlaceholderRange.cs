using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(RangeOrPlaceholderRangeConverter))]
    public class RangeOrPlaceholderRange
    {
        private RenameDefaultBehavior? _renameDefaultBehavior;
        private Range? _range;
        private PlaceholderRange? _placeholderRange;

        public RangeOrPlaceholderRange(Range value)
        {
            _range = value;
        }

        public RangeOrPlaceholderRange(PlaceholderRange value)
        {
            _placeholderRange = value;
        }

        public RangeOrPlaceholderRange(RenameDefaultBehavior renameDefaultBehavior)
        {
            _renameDefaultBehavior = renameDefaultBehavior;
        }

        public bool IsPlaceholderRange => _placeholderRange != null;

        public PlaceholderRange? PlaceholderRange
        {
            get => _placeholderRange;
            set {
                _placeholderRange = value;
                _renameDefaultBehavior = default;
                _range = null;
            }
        }

        public bool IsRange => _range is not null;

        public Range? Range
        {
            get => _range;
            set {
                _placeholderRange = default;
                _renameDefaultBehavior = default;
                _range = value;
            }
        }

        public bool IsDefaultBehavior => _renameDefaultBehavior is not null;

        public RenameDefaultBehavior? DefaultBehavior
        {
            get => _renameDefaultBehavior;
            set {
                _placeholderRange = default;
                _renameDefaultBehavior = value;
                _range = default;
            }
        }

        public object? RawValue
        {
            get {
                if (IsPlaceholderRange) return PlaceholderRange;
                if (IsRange) return Range;
                if (IsDefaultBehavior) return DefaultBehavior;
                return default;
            }
        }

        public static implicit operator RangeOrPlaceholderRange(PlaceholderRange value) => new RangeOrPlaceholderRange(value);

        public static implicit operator RangeOrPlaceholderRange(Range value) => new RangeOrPlaceholderRange(value);
    }
}
