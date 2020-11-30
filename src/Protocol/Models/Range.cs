using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public partial record Range
    {
        public Range()
        {
        }

        public Range(Position start, Position end)
        {
            Start = start;
            End = end;
        }

        public Range(int startLine, int startCharacter, int endLine, int endCharacter)
        {
            Start = ( startLine, startCharacter );
            End = ( endLine, endCharacter );
        }

        /// <summary>
        /// The range's start position.
        /// </summary>
        public Position Start { get; init; }

        /// <summary>
        /// The range's end position.
        /// </summary>
        public Position End { get; init; }

        public static implicit operator Range((Position start, Position end) value) => new Range(value.start, value.end);

        /// <inheritdoc />
        public override string ToString() => $"[start: ({Start?.Line}, {Start?.Character}), end: ({End?.Line}, {End?.Character})]";
    }
}
