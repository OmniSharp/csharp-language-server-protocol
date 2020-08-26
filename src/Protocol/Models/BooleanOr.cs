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
            _value = default!;
            _bool = value;
        }

        // To avoid boxing, the best way to compare generics for equality is with EqualityComparer<T>.Default.
        // This respects IEquatable<T> (without boxing) as well as object.Equals, and handles all the Nullable<T> "lifted" nuances.
        // https://stackoverflow.com/a/864860
        public bool IsValue => !EqualityComparer<T>.Default.Equals(_value, default!);

        public T Value
        {
            get => _value;
            set {
                _value = value;
                _bool = null;
            }
        }

        public bool IsBool => _bool.HasValue;

        public bool Bool
        {
            get => _bool.HasValue && _bool.Value;
            set {
                _value = default!;
                _bool = value;
            }
        }

        public object? RawValue
        {
            get {
                if (IsBool) return Bool;
                if (IsValue) return Value;
                return null;
            }
        }

        public static implicit operator BooleanOr<T>(T value) => value != null ? new BooleanOr<T>(value) : new BooleanOr<T>(false);

        public static implicit operator BooleanOr<T>(bool value) => new BooleanOr<T>(value);
    }
}
