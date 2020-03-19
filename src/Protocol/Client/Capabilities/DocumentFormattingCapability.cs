using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentFormattingCapability : DynamicCapability, ConnectedCapability<IDocumentFormattingHandler> { }
}
