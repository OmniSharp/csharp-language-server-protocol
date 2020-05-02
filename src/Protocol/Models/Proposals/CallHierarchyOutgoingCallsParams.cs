using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// The parameter of a `callHierarchy/outgoingCalls` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(DocumentNames.CallHierarchyOutgoing)]
    public class CallHierarchyOutgoingCallsParams  : IWorkDoneProgressParams, IPartialItems<CallHierarchyItem>, IRequest<Container<CallHierarchyOutgoingCall>>
    {
        public CallHierarchyItem Item { get; set; }
        public ProgressToken WorkDoneToken { get; set; }
        public ProgressToken PartialResultToken { get; set; }
    }
}
