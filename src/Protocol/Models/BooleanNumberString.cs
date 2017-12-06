using Newtonsoft.Json;

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

        public bool IsLong => this._long.HasValue;
        public long Long
        {
            get { return _long ?? 0; }
            set
            {
                this.String = null;
                this._long = value;
                this._bool = null;
            }
        }

        public bool IsString => this._string != null;
        public string String
        {
            get { return this._string; }
            set
            {
                this._string = value;
                this._long = null;
                this._bool = null;
            }
        }

        public bool IsBool => this._bool.HasValue;
        public bool Bool
        {
            get { return this._bool.HasValue && this._bool.Value; }
            set
            {
                this.String = null;
                this._long = null;
                this._bool = value;
            }
        }
        public object Value
        {
            get
            {
                if (IsBool) return Bool;
                if (IsLong) return Long;
                if (IsString) return String;
                return null;
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
