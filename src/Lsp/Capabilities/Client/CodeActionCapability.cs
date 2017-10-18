using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    public class CodeActionCapability : DynamicCapability, ConnectedCapability<ICodeActionHandler> { }
}
