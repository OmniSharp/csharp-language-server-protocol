using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentRangeFormattingClientCapabilities : DynamicCapability, ConnectedCapability<IDocumentRangeFormattingHandler> { }
}
