using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class BooleanOr<T>
    {
        private T _value;
        private bool? _bool;
        public BooleanOr(T value)
        {
            _value = value;
            _bool = null;
        }
        public BooleanOr(bool value)
        {
            _value = default;
            _bool = value;
        }

        // To avoid boxing, the best way to compare generics for equality is with EqualityComparer<T>.Default.
        // This respects IEquatable<T> (without boxing) as well as object.Equals, and handles all the Nullable<T> "lifted" nuances.
        // https://stackoverflow.com/a/864860
        public bool IsValue => !EqualityComparer<T>.Default.Equals(_value, default);

        public T Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this._bool = null;
            }
        }

        public bool IsBool => this._bool.HasValue;
        public bool Bool
        {
            get { return this._bool.HasValue && this._bool.Value; }
            set
            {
                this.Value = default;
                this._bool = value;
            }
        }
        public object RawValue
        {
            get
            {
                if (IsBool) return Bool;
                if (IsValue) return Value;
                return null;
            }
        }

        public static implicit operator BooleanOr<T>(T value)
        {
            return value != null ? new BooleanOr<T>(value) : null;
        }

        public static implicit operator BooleanOr<T>(bool value)
        {
            return new BooleanOr<T>(value);
        }
    }
}
