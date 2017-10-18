using OmniSharp.Extensions.LanguageServer.Capabilities.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    public class CodeLensCapability : DynamicCapability, ConnectedCapability<ICodeLensHandler> { }
}
