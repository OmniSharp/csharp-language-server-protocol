using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ColorProviderCapability : DynamicCapability, ConnectedCapability<IDocumentColorHandler>, ConnectedCapability<IColorPresentationHandler> { }
}
