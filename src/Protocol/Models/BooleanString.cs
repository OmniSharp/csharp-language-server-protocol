using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(BooleanStringConverter))]
    public struct BooleanString
    {
        private string? _string;
        private bool? _bool;

        public BooleanString(Guid value)
        {
            _string = value.ToString();
            _bool = null;
        }

        public BooleanString(string value)
        {
            _string = value;
            _bool = null;
        }

        public BooleanString(bool value)
        {
            _string = null;
            _bool = value;
        }

        public bool IsString => _string != null;

        public string String
        {
            get => _string ?? string.Empty;
            set {
                _string = value;
                _bool = null;
            }
        }

        public bool IsBool => _bool.HasValue;

        public bool Bool
        {
            get => _bool.HasValue && _bool.Value;
            set {
                _string = null;
                _bool = value;
            }
        }

        public static implicit operator BooleanString(string value) => new BooleanString(value);
        public static implicit operator BooleanString(Guid value) => new BooleanString(value.ToString());

        public static implicit operator BooleanString(bool value) => new BooleanString(value);
    }
}
