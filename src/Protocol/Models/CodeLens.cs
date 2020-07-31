using System.ComponentModel;
using System.Diagnostics;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

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
    public class CodeLens : ICanBeResolved, IRequest<CodeLens>
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The command this code lens represents.
        /// </summary>
        [Optional]
        public Command Command { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public JToken Data { get; set; }

        private string DebuggerDisplay => $"{Range}{(Command != null ? $" Command" : "")}";
        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;

        /// <summary>
        /// Convert from a <see cref="CodeLens"/>
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal CodeLens<T> From<T>(ISerializer serializer) where T : class
        {
            return new CodeLens<T>() {
                Command = Command,
                Data = Data?.ToObject<T>(serializer.JsonSerializer),
                Range = Range
            };
        }
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
    public class CodeLens<T> : CodeLens where T : class
    {
        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        public new T Data { get; set; }

        /// <summary>
        /// Convert to a <see cref="CodeLens"/>
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal CodeLens To(ISerializer serializer)
        {
            if (Data != null)
            {
                base.Data = JObject.FromObject(Data, serializer.JsonSerializer);
            }

            return this;
        }
    }
}
