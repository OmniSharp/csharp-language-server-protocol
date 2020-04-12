using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Client.Proposals
{
    [Obsolete(Constants.Proposal)]
    public static class CallHierarchyExtensions
    {
        public static Task<Container<CallHierarchyItem>> PrepareCallHierarchy(this ILanguageClientDocument mediator,
            CallHierarchyPrepareParams @params)
        {
            return mediator.SendRequest<CallHierarchyPrepareParams, Container<CallHierarchyItem>>(
                DocumentNames.PrepareCallHierarchy, @params);
        }

        public static Task<Container<CallHierarchyIncomingCall>> CallHierarchyIncomingCalls(
            this ILanguageClientDocument mediator, CallHierarchyIncomingCallsParams @params)
        {
            return mediator.SendRequest<CallHierarchyIncomingCallsParams, Container<CallHierarchyIncomingCall>>(
                DocumentNames.CallHierarchyIncoming, @params);
        }

        public static Task<Container<CallHierarchyOutgoingCall>> CallHierarchyOutgoingCalls(
            this ILanguageClientDocument mediator, CallHierarchyOutgoingCallsParams @params)
        {
            return mediator.SendRequest<CallHierarchyOutgoingCallsParams, Container<CallHierarchyOutgoingCall>>(
                DocumentNames.CallHierarchyIncoming, @params);
        }
    }
}
