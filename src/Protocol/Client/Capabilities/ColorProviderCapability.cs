namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ColorProviderCapability : DynamicCapability, ConnectedCapability<IDocumentColorHandler>, ConnectedCapability<IColorPresentationHandler> {}
}
