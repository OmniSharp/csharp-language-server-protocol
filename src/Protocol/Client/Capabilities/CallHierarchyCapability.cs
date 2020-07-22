using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the `textDocument/callHierarchy`.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyCapability : DynamicCapability, ConnectedCapability<ICallHierarchyHandler>,
        ConnectedCapability<ICallHierarchyIncomingHandler>, ConnectedCapability<ICallHierarchyOutgoingHandler>
    {
    }
}
