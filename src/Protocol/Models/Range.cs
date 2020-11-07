using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public partial record Range(Position Start, Position End)
    {
        public Range() :this((0, 0), (0, 0))
        {
        }

        public Range(int startLine, int startCharacter, int endLine, int endCharacter) : this(( startLine, startCharacter ), ( endLine, endCharacter ))
        {
        }

        public static implicit operator Range((Position start, Position end) value) => new Range(value.start, value.end);

        public void Deconstruct(out Position start, out Position end)
        {
            start = Start;
            end = End;
        }

        public void Deconstruct(out int startLine, out int startCharacter, out int endLine, out int endCharacter)
        {
            startLine = Start.Line;
            startCharacter = Start.Character;
            endLine = End.Line;
            endCharacter = End.Character;
        }

        private string DebuggerDisplay => $"[start: ({Start?.Line}, {Start?.Character}), end: ({End?.Line}, {End?.Character})]";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
