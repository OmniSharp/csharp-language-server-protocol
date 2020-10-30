using System;
using Newtonsoft.Json.Linq;
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
    public class CallHierarchyIncomingCallsParams : IWorkDoneProgressParams, ICanBeResolved, IPartialItemsRequest<Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall>
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
    /// The parameter of a `callHierarchy/incomingCalls` request.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.CallHierarchyIncoming, Direction.ClientToServer)]
    public class CallHierarchyIncomingCallsParams<T> : IWorkDoneProgressParams, ICanBeResolved,
                                                       IPartialItemsRequest<Container<CallHierarchyIncomingCall>?, CallHierarchyIncomingCall> where T : HandlerIdentity?, new()
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
