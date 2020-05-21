using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{

    [Parallel, Method(TextDocumentNames.PublishDiagnostics, Direction.ServerToClient)]
    public interface IPublishDiagnosticsHandler : IJsonRpcNotificationHandler<PublishDiagnosticsParams> { }

    public abstract class PublishDiagnosticsHandler : IPublishDiagnosticsHandler
    {
        public abstract Task<Unit> Handle(PublishDiagnosticsParams request, CancellationToken cancellationToken);
    }

    public static class PublishDiagnosticsExtensions
    {
        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry,
            Func<PublishDiagnosticsParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(TextDocumentNames.PublishDiagnostics, NotificationHandler.For(handler));
        }

        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry,
            Func<PublishDiagnosticsParams, Task> handler)
        {
            return registry.AddHandler(TextDocumentNames.PublishDiagnostics, NotificationHandler.For(handler));
        }

        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry,
            Action<PublishDiagnosticsParams, CancellationToken> handler)
        {
            return registry.AddHandler(TextDocumentNames.PublishDiagnostics, NotificationHandler.For(handler));
        }

        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry,
            Action<PublishDiagnosticsParams> handler)
        {
            return registry.AddHandler(TextDocumentNames.PublishDiagnostics, NotificationHandler.For(handler));
        }

        public static void PublishDiagnostics(this ITextDocumentLanguageServer mediator, PublishDiagnosticsParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
