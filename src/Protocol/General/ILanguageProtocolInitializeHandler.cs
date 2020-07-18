using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Serial, Method(GeneralNames.Initialize, Direction.ClientToServer)]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry), MethodName = "OnLanguageProtocolInitialize"), GenerateRequestMethods(typeof(ILanguageClient), MethodName = "RequestLanguageProtocolInitialize")]
    public interface ILanguageProtocolInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult>
    {
    }

    public abstract class LanguageProtocolInitializeHandler : ILanguageProtocolInitializeHandler
    {
        public abstract Task<InitializeResult> Handle(InitializeParams request, CancellationToken cancellationToken);
    }
}
