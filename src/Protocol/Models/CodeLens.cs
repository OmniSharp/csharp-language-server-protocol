using System.Diagnostics;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    public class CodeLens : IRequest<CodeLens>, ICanBeResolved
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The command this code lens represents.
        /// </summary>
        [Optional]
        public Command? Command { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public JToken? Data { get; set; }

        private string DebuggerDisplay => $"{Range}{( Command != null ? $" {Command}" : "" )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }

    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    /// <remarks>
    /// Typed code lens used for the typed handlers
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class CodeLens<T> : ICanBeResolved
        where T : HandlerIdentity?, new()
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The command this code lens represents.
        /// </summary>
        [Optional]
        public Command? Command { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        public T Data
        {
            get => ( (ICanBeResolved) this ).Data?.ToObject<T>()!;
            set => ( (ICanBeResolved) this ).Data = JToken.FromObject(value);
        }

        JToken? ICanBeResolved.Data { get; set; }

        public static implicit operator CodeLens(CodeLens<T> value) => new CodeLens {
            Data = ( (ICanBeResolved) value ).Data,
            Command = value.Command,
            Range = value.Range,
        };

        public static implicit operator CodeLens<T>(CodeLens value)
        {
            var item = new CodeLens<T> {
                Command = value.Command,
                Range = value.Range,
            };
            ( (ICanBeResolved) item ).Data = value.Data;
            return item;
        }

        private string DebuggerDisplay => $"{Range}{( Command != null ? $" {Command}" : "" )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
