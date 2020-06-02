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
    public interface ICodeLens<out TData> : ICanBeResolved<TData> where TData : CanBeResolvedData
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        Range Range { get; }

        /// <summary>
        /// The command this code lens represents.
        /// </summary>
        [Optional]
        Command Command { get; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        TData Data { get; }
    }

    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    [Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    public class CodeLens<TData> : ICodeLens<TData>, IRequest<CodeLens<TData>>
        where TData : CanBeResolvedData
    {
        /// <summary>
        /// Used for aggregating results when completion is supported by multiple handlers
        /// </summary>
        public static CodeLens<TData> From(ICodeLens<TData> item)
        {
            return item is CodeLens<TData> cl
                ? cl
                : new CodeLens<TData>() {
                    Command = item.Command,
                    Data = item.Data,
                    Range = item.Range
                };
        }

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
        public TData Data { get; set; }
    }
}
