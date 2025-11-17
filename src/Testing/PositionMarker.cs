namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    public readonly struct PositionMarker : IEquatable<PositionMarker>, IComparable<PositionMarker>, IComparable
    {
        public char First { get; }
        public char Second { get; }
        public int CompareTo(PositionMarker other)
        {
            var firstComparison = First.CompareTo(other.First);
            if (firstComparison != 0) return firstComparison;
            return Second.CompareTo(other.Second);
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is PositionMarker other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(PositionMarker)}");
        }

        public static bool operator <(PositionMarker left, PositionMarker right) => left.CompareTo(right) < 0;

        public static bool operator >(PositionMarker left, PositionMarker right) => left.CompareTo(right) > 0;

        public static bool operator <=(PositionMarker left, PositionMarker right) => left.CompareTo(right) <= 0;

        public static bool operator >=(PositionMarker left, PositionMarker right) => left.CompareTo(right) >= 0;

        public bool Equals(PositionMarker other) => First == other.First && Second == other.Second;

        public override bool Equals(object? obj) => obj is PositionMarker other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ( First.GetHashCode() * 397 ) ^ Second.GetHashCode();
            }
        }

        public static bool operator ==(PositionMarker left, PositionMarker right) => left.Equals(right);

        public static bool operator !=(PositionMarker left, PositionMarker right) => !left.Equals(right);


        public PositionMarker(char first, char second)
        {
            First = first;
            Second = second;
        }

        public PositionMarker(string value)
        {
            if (value.Length != 2) throw new ArgumentOutOfRangeException(nameof(value), value, "Expected a string with 2 characters");
            First = value[0];
            Second = value[1];
        }

        public void Deconstruct(out char first, out char second)
        {
            first = First;
            second = Second;
        }

        public static implicit operator PositionMarker((char first, char second) value)
        {
            return new PositionMarker(value.first, value.second);
        }

        public override string ToString() => $"{First}{Second}";
    }
}