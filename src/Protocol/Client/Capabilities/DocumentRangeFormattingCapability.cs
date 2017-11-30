using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentRangeFormattingCapability : DynamicCapability, ConnectedCapability<IDocumentRangeFormattingHandler> { }
}
