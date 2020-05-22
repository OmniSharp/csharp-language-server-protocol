using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
    public interface IRegisterCapabilityHandler : IJsonRpcRequestHandler<RegistrationParams>
    {
    }

    public abstract class RegisterCapabilityHandler : IRegisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(RegistrationParams request, CancellationToken cancellationToken);
    }

    public static class RegisterCapabilityExtensions
    {
        public static IDisposable OnRegisterCapability(this ILanguageClientRegistry registry,
            Func<RegistrationParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(ClientNames.RegisterCapability, RequestHandler.For(handler));
        }

        public static IDisposable OnRegisterCapability(this ILanguageClientRegistry registry,
            Func<RegistrationParams, Task> handler)
        {
            return registry.AddHandler(ClientNames.RegisterCapability, RequestHandler.For(handler));
        }

        public static Task RegisterCapability(this IClientLanguageServer mediator, RegistrationParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
