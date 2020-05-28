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
    public interface ILanguageProtocolInitializedHandler : IJsonRpcNotificationHandler<InitializedParams> { }

    public abstract class LanguageProtocolInitializedHandler : ILanguageProtocolInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken);
    }

    public static class LanguageProtocolInitializedExtensions
    {
        public static ILanguageServerRegistry OnLanguageProtocolInitialized(this ILanguageServerRegistry registry, Action<InitializedParams> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static ILanguageServerRegistry OnLanguageProtocolInitialized(this ILanguageServerRegistry registry, Action<InitializedParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static ILanguageServerRegistry OnLanguageProtocolInitialized(this ILanguageServerRegistry registry, Func<InitializedParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static ILanguageServerRegistry OnLanguageProtocolInitialized(this ILanguageServerRegistry registry, Func<InitializedParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Initialized, NotificationHandler.For(handler));
        }

        public static void SendLanguageProtocolInitialized(this ILanguageClient mediator, InitializedParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
