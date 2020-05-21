using System;
using MediatR;
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
    public class CallHierarchyIncomingCallsParams : IWorkDoneProgressParams, IPartialItemsRequest<CallHierarchyIncomingCall>
    {
        public CallHierarchyItem Item { get; set; }
        public ProgressToken WorkDoneToken { get; set; }
        public ProgressToken PartialResultToken { get; set; }
    }
}
