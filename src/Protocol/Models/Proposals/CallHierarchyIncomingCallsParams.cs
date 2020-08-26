using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// The parameter of a `callHierarchy/incomingCalls` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.CallHierarchyIncoming, Direction.ClientToServer)]
    public class CallHierarchyIncomingCallsParams : IWorkDoneProgressParams, IPartialItemsRequest<Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall>
    {
        public CallHierarchyItem Item { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; }
        public ProgressToken? PartialResultToken { get; set; }
    }
}
