using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [Obsolete(Constants.Proposal)]
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Moniker))]
    public class MonikerCapability : DynamicCapability { }
}
