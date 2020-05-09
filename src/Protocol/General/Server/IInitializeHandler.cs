using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Serial, Method(GeneralNames.Initialize)]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> { }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<InitializeResult> Handle(InitializeParams request, CancellationToken cancellationToken);
    }

    public static class InitializeHandlerExtensions
    {
        public static IDisposable OnInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, CancellationToken, Task<InitializeResult>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : InitializeHandler
        {
            private readonly Func<InitializeParams, CancellationToken, Task<InitializeResult>> _handler;

            public DelegatingHandler(Func<InitializeParams, CancellationToken, Task<InitializeResult>> handler)
            {
                _handler = handler;
            }

            public override Task<InitializeResult> Handle(InitializeParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);

        }
    }
}
