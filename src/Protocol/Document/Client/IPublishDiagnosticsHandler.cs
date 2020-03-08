using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{

    [Parallel, Method(DocumentNames.PublishDiagnostics)]
    public interface IPublishDiagnosticsHandler : IJsonRpcNotificationHandler<PublishDiagnosticsParams> { }

    public abstract class PublishDiagnosticsHandler : IPublishDiagnosticsHandler
    {
        public abstract Task<Unit> Handle(PublishDiagnosticsParams request, CancellationToken cancellationToken);
    }

    public static class PublishDiagnosticsHandlerExtensions
    {
        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry, Func<PublishDiagnosticsParams, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : PublishDiagnosticsHandler
        {
            private readonly Func<PublishDiagnosticsParams, Task<Unit>> _handler;

            public DelegatingHandler(Func<PublishDiagnosticsParams, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(PublishDiagnosticsParams request, CancellationToken cancellationToken) => _handler.Invoke(request);
        }
    }
}
