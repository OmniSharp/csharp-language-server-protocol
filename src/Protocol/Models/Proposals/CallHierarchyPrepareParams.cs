using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// The parameter of a `textDocument/prepareCallHierarchy` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.PrepareCallHierarchy, Direction.ClientToServer)]
    public class CallHierarchyPrepareParams : WorkDoneTextDocumentPositionParams, IRequest<Container<CallHierarchyItem>>
    {
    }
}
