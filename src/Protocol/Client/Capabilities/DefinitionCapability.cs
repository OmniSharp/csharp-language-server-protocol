using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DefinitionCapability : LinkSupportCapability, ConnectedCapability<IDefinitionHandler> { }
}
