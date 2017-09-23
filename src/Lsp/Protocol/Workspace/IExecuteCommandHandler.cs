using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Method("workspace/executeCommand")]
    public interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions>, ICapability<ExecuteCommandCapability> { }
}