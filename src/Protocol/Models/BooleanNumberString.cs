using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(BooleanNumberStringConverter))]
    public struct BooleanNumberString
    {
        private int? _int;
        private string? _string;
        private bool? _bool;

        public BooleanNumberString(int value)
        {
            _int = value;
            _string = null;
            _bool = null;
        }

        public BooleanNumberString(string value)
        {
            _int = null;
            _string = value;
            _bool = null;
        }

        public BooleanNumberString(bool value)
        {
            _int = null;
            _string = null;
            _bool = value;
        }

        public bool IsInteger => _int.HasValue;

        public int Integer
        {
            get => _int ?? 0;
            set {
                _string = null;
                _int = value;
                _bool = null;
            }
        }

        public bool IsString => _string != null;

        public string String
        {
            get => _string ?? string.Empty;
            set {
                _string = value;
                _int = null;
                _bool = null;
            }
        }

        public bool IsBool => _bool.HasValue;

        public bool Bool
        {
            get => _bool.HasValue && _bool.Value;
            set {
                _string = null;
                _int = null;
                _bool = value;
            }
        }

        public static implicit operator BooleanNumberString(int value) => new BooleanNumberString(value);

        public static implicit operator BooleanNumberString(string value) => new BooleanNumberString(value);

        public static implicit operator BooleanNumberString(bool value) => new BooleanNumberString(value);
    }
}
