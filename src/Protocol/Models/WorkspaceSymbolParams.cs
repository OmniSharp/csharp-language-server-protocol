using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The parameters of a Workspace Symbol Request.
    /// </summary>
    [Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
    public class WorkspaceSymbolParams : IPartialItemsRequest<Container<SymbolInformation>?, SymbolInformation>, IWorkDoneProgressParams
    {
        /// <summary>
        /// A non-empty query string
        /// </summary>
        public string Query { get; set; } = null!;

        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
