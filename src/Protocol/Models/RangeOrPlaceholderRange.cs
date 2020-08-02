using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(RangeOrPlaceholderRangeConverter))]
    public class RangeOrPlaceholderRange
    {

        private Range _range;
        private PlaceholderRange _placeholderRange;
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

        public bool IsPlaceholderRange => this._placeholderRange != null;
        public PlaceholderRange PlaceholderRange
        {
            get { return this._placeholderRange; }
            set
            {
                this._placeholderRange = value;
                this._range = null;
            }
        }

        public bool IsRange => this._range != null;
        public Range Range
        {
            get { return this._range; }
            set
            {
                this._placeholderRange = default;
                this._range = value;
            }
        }
        public object RawValue
        {
            get
            {
                if (IsPlaceholderRange) return PlaceholderRange;
                if (IsRange) return Range;
                return default;
            }
        }

        public static implicit operator RangeOrPlaceholderRange(PlaceholderRange value)
        {
            return new RangeOrPlaceholderRange(value);
        }

        public static implicit operator RangeOrPlaceholderRange(Range value)
        {
            return new RangeOrPlaceholderRange(value);
        }
    }
}
