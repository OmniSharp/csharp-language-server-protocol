using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    public class DocumentOnTypeFormattingCapability : DynamicCapability, ConnectedCapability<IDocumentOnTypeFormatHandler> { }
}
