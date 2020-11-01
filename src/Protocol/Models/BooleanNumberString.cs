using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(BooleanNumberStringConverter))]
    public struct BooleanNumberString
    {
        private long? _long;
        private string? _string;
        private bool? _bool;

        public BooleanNumberString(long value)
        {
            _long = value;
            _string = null;
            _bool = null;
        }

        public BooleanNumberString(string value)
        {
            _long = null;
            _string = value;
            _bool = null;
        }

        public BooleanNumberString(bool value)
        {
            _long = null;
            _string = null;
            _bool = value;
        }

        public bool IsLong => _long.HasValue;

        public long Long
        {
            get => _long ?? 0;
            set {
                _string = null;
                _long = value;
                _bool = null;
            }
        }

        public bool IsString => _string != null;

        public string String
        {
            get => _string ?? string.Empty;
            set {
                _string = value;
                _long = null;
                _bool = null;
            }
        }

        public bool IsBool => _bool.HasValue;

        public bool Bool
        {
            get => _bool.HasValue && _bool.Value;
            set {
                _string = null;
                _long = null;
                _bool = value;
            }
        }

        public static implicit operator BooleanNumberString(long value) => new BooleanNumberString(value);

        public static implicit operator BooleanNumberString(string value) => new BooleanNumberString(value);

        public static implicit operator BooleanNumberString(bool value) => new BooleanNumberString(value);
    }
}
