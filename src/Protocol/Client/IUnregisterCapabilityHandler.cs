using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(ClientNames.UnregisterCapability, Direction.ServerToClient)]
    public interface IUnregisterCapabilityHandler : IJsonRpcRequestHandler<UnregistrationParams> { }

    public abstract class UnregisterCapabilityHandler : IUnregisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(UnregistrationParams request, CancellationToken cancellationToken);
    }

    public static class UnregisterCapabilityExtensions
    {
        public static IDisposable OnUnregisterCapability(this ILanguageClientRegistry registry,
            Func<UnregistrationParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(ClientNames.UnregisterCapability, RequestHandler.For(handler));
        }

        public static IDisposable OnUnregisterCapability(this ILanguageClientRegistry registry,
            Func<UnregistrationParams, Task> handler)
        {
            return registry.AddHandler(ClientNames.UnregisterCapability, RequestHandler.For(handler));
        }

        public static Task UnregisterCapability(this IClientLanguageServer mediator, UnregistrationParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
