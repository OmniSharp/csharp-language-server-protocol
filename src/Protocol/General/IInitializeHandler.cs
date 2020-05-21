using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Serial, Method(GeneralNames.Initialize, Direction.ClientToServer)]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> { }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<InitializeResult> Handle(InitializeParams request, CancellationToken cancellationToken);
    }

    public static class InitializeExtensions
    {
        public static IDisposable OnInitialize(
            this ILanguageServerRegistry registry,
            Func<InitializeParams, CancellationToken, Task<InitializeResult>>
                handler)
        {
            return registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        }

        public static IDisposable OnInitialize(
            this ILanguageServerRegistry registry,
            Func<InitializeParams, Task<InitializeResult>> handler)
        {
            return registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        }

        public static Task<InitializeResult> RequestInitialize(this ILanguageClient mediator, InitializeParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
