using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// The parameter of a `callHierarchy/outgoingCalls` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.CallHierarchyOutgoing, Direction.ClientToServer)]
    public class CallHierarchyOutgoingCallsParams : IWorkDoneProgressParams, IPartialItemsRequest<Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall>
    {
        public CallHierarchyItem Item { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; }
        public ProgressToken? PartialResultToken { get; set; }
    }
}
