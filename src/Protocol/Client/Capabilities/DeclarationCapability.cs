using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DeclarationCapability : LinkSupportCapability, ConnectedCapability<IDeclarationHandler>
    {
    }
}
