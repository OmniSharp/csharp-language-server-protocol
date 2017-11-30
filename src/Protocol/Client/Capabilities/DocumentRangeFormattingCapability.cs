using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    public class DocumentRangeFormattingCapability : DynamicCapability, ConnectedCapability<IDocumentRangeFormattingHandler> { }
}
