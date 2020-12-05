using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public partial record Position : IComparable<Position>, IComparable
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
        /// <remarks>
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        public int Line { get; set; }

        /// <summary>
        /// Character offset on a line in a document (zero-based).
        /// </summary>
        /// <remarks>
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        public int Character { get; set; }

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

        public static implicit operator Position((int line, int character) value) => new Position(value.line, value.character);

        public static bool operator <(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) < 0;

        public static bool operator >(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) > 0;

        public static bool operator <=(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) <= 0;

        public static bool operator >=(Position left, Position right) => Comparer<Position>.Default.Compare(left, right) >= 0;

        /// <inheritdoc />
        public override string ToString() => $"(line: {Line}, char: {Character})";
    }
}
