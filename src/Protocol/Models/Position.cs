using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public partial record Position(int Line, int Character) : IComparable<Position>, IComparable
    {
        public static Position Zero => ( 0, 0 );

        public Position() : this(0, 0)
        {
        }

        public int CompareTo(Position? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var lineComparison = Line.CompareTo(other.Line);
            return lineComparison != 0 ? lineComparison : Character.CompareTo(other.Character);
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is Position other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Position)}");
        }

        public override int GetHashCode()
        {
            var hashCode = 1927683087;
            hashCode = hashCode * -1521134295 + Line.GetHashCode();
            hashCode = hashCode * -1521134295 + Character.GetHashCode();
            return hashCode;
        }

        public static implicit operator Position((int line, int character) value) => new Position(value.line, value.character);

        public void Deconstruct(out int line, out int character)
        {
            line = Line;
            character = Character;
        }

        public static bool operator <(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) < 0;

        public static bool operator >(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) > 0;

        public static bool operator <=(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) <= 0;

        public static bool operator >=(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) >= 0;

        private string DebuggerDisplay => $"(line: {Line}, char: {Character})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
