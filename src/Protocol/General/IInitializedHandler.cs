using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    [Serial, Method(GeneralNames.Initialized, Direction.ClientToServer)]
    public interface IInitializedHandler : IJsonRpcNotificationHandler<InitializedParams> { }

    public abstract class InitializedHandler : IInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken);
    }

    public static class InitializedExtensions
    {
        public static IDisposable OnInitialized(this ILanguageServerRegistry registry, Action<InitializedParams> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static IDisposable OnInitialized(this ILanguageServerRegistry registry, Action<InitializedParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static IDisposable OnInitialized(this ILanguageServerRegistry registry, Func<InitializedParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static IDisposable OnInitialized(this ILanguageServerRegistry registry, Func<InitializedParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static void SendInitialized(this ILanguageClient mediator, InitializedParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
