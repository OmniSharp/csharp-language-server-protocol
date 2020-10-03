using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Position : IEquatable<Position>, IComparable<Position>, IComparable
    {
        public Position()
        {
        }

        public Position(int line, int character)
        {
            Line = line;
            Character = character;
        }

        /// <summary>
        /// Line position in a document (zero-based).
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Character offset on a line in a document (zero-based).
        /// </summary>
        public int Character { get; set; }

        public override bool Equals(object obj) => Equals(obj as Position);

        public bool Equals(Position other) =>
            other != null &&
            Line == other.Line &&
            Character == other.Character;

        public int CompareTo(Position other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var lineComparison = Line.CompareTo(other.Line);
            return lineComparison != 0 ? lineComparison : Character.CompareTo(other.Character);
        }

        public int CompareTo(object obj)
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

        public static bool operator ==(Position position1, Position position2) => EqualityComparer<Position>.Default.Equals(position1, position2);

        public static bool operator !=(Position position1, Position position2) => !( position1 == position2 );

        public static implicit operator Position((int line, int character) value) => new Position(value.line, value.character);

        public static bool operator <(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) < 0;

        public static bool operator >(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) > 0;

        public static bool operator <=(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) <= 0;

        public static bool operator >=(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) >= 0;

        private string DebuggerDisplay => $"(line: {Line}, char: {Character})";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
