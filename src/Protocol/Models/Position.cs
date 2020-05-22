using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class Position : IEquatable<Position>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        public bool Equals(Position other)
        {
            return other != null &&
                   Line == other.Line &&
                   Character == other.Character;
        }

        public override int GetHashCode()
        {
            var hashCode = 1927683087;
            hashCode = hashCode * -1521134295 + Line.GetHashCode();
            hashCode = hashCode * -1521134295 + Character.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Position position1, Position position2)
        {
            return EqualityComparer<Position>.Default.Equals(position1, position2);
        }

        public static bool operator !=(Position position1, Position position2)
        {
            return !(position1 == position2);
        }

        public static implicit operator Position((int line, int character) value)
        {
            return new Position(value.line, value.character);
        }
    }
}
