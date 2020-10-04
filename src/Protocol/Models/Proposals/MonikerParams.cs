using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Language Server Index Format (LSIF) introduced the concept of symbol monikers to help associate symbols across different indexes.
    /// This request adds capability for LSP server implementations to provide the same symbol moniker information given a text document
    /// position. Clients can utilize this method to get the moniker at the current location in a file user is editing and do further
    /// code navigation queries in other services that rely on LSIF indexes and link symbols together.
    ///
    /// The `textDocument/moniker` request is sent from the client to the server to get the symbol monikers for a given text document
    /// position. An array of Moniker types is returned as response to indicate possible monikers at the given location. If no monikers
    /// can be calculated, an empty array or `null` should be returned.
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.Moniker, Direction.ClientToServer)]
    public class MonikerParams : WorkDoneTextDocumentPositionParams, IPartialItemsRequest<Container<Moniker>, Moniker>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }
    }
}
