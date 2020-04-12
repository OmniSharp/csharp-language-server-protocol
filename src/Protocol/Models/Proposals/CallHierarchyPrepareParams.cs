using System;
using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// The parameter of a `textDocument/prepareCallHierarchy` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyPrepareParams : WorkDoneTextDocumentPositionParams, IRequest<Container<CallHierarchyItem>>
    {
    }
}
