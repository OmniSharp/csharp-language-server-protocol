using System;
using Newtonsoft.Json.Linq;
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
    public class CallHierarchyOutgoingCallsParams : IWorkDoneProgressParams, ICanBeResolved, IPartialItemsRequest<Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall>
    {
        public CallHierarchyItem Item { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; }
        public ProgressToken? PartialResultToken { get; set; }
        JToken? ICanBeResolved.Data
        {
            get => ( (ICanBeResolved) Item )?.Data;
            set {
                if (Item != null) ( (ICanBeResolved) Item ).Data = value;
            }
        }
    }

    /// <summary>
    /// The parameter of a `callHierarchy/outgoingCalls` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.CallHierarchyOutgoing, Direction.ClientToServer)]
    public class CallHierarchyOutgoingCallsParams<T> : IWorkDoneProgressParams, ICanBeResolved, IPartialItemsRequest<Container<CallHierarchyOutgoingCall>?, CallHierarchyOutgoingCall> where T : HandlerIdentity?, new()
    {
        public CallHierarchyItem<T> Item { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; }
        public ProgressToken? PartialResultToken { get; set; }
        JToken? ICanBeResolved.Data
        {
            get => ( (ICanBeResolved) Item )?.Data;
            set {
                if (Item != null) ( (ICanBeResolved) Item ).Data = value;
            }
        }
    }
}
