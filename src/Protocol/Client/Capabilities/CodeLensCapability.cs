using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CodeLensCapability : DynamicCapability, ConnectedCapability<ICodeLensHandler<CanBeResolvedData>> { }
}
