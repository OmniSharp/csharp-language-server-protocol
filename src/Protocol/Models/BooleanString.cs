namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public struct BooleanString
    {
        private string _string;
        private bool? _bool;
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

        public bool IsString => this._string != null;
        public string String
        {
            get { return this._string; }
            set
            {
                this._string = value;
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
                this._bool = value;
            }
        }
        public object Value
        {
            get
            {
                if (IsBool) return Bool;
                if (IsString) return String;
                return null;
            }
        }

        public static implicit operator BooleanString(string value)
        {
            return new BooleanString(value);
        }

        public static implicit operator BooleanString(bool value)
        {
            return new BooleanString(value);
        }
    }
}
