namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public struct BooleanNumberString
    {
        private long? _long;
        private string _string;
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
            set
            {
                String = null;
                _long = value;
                _bool = null;
            }
        }

        public bool IsString => _string != null;
        public string String
        {
            get => _string;
            set
            {
                _string = value;
                _long = null;
                _bool = null;
            }
        }

        public bool IsBool => _bool.HasValue;
        public bool Bool
        {
            get => _bool.HasValue && _bool.Value;
            set
            {
                String = null;
                _long = null;
                _bool = value;
            }
        }

        public static implicit operator BooleanNumberString(long value)
        {
            return new BooleanNumberString(value);
        }

        public static implicit operator BooleanNumberString(string value)
        {
            return new BooleanNumberString(value);
        }

        public static implicit operator BooleanNumberString(bool value)
        {
            return new BooleanNumberString(value);
        }
    }
}
