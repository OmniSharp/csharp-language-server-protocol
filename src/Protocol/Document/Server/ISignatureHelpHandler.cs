using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static DocumentNames;
    [Parallel, Method(SignatureHelp)]
    public interface ISignatureHelpHandler : IJsonRpcRequestHandler<SignatureHelpParams, SignatureHelp>, IRegistration<SignatureHelpRegistrationOptions>, ICapability<SignatureHelpCapability> { }
}
