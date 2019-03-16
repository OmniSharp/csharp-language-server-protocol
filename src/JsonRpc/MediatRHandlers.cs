using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class MediatRHandlers
    {
        private static readonly MethodInfo SendRequestUnit = typeof(MediatRHandlers)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(x => x.Name == nameof(SendRequest))
            .First(x => x.GetGenericArguments().Length == 1);

        private static readonly MethodInfo SendRequestResponse = typeof(MediatRHandlers)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(x => x.Name == nameof(SendRequest))
            .First(x => x.GetGenericArguments().Length == 2);

        public static Task HandleNotification(IMediator mediator, IHandlerDescriptor handler, object @params, CancellationToken token)
        {
            return (Task)SendRequestUnit
                .MakeGenericMethod(handler.Params ?? typeof(EmptyRequest))
                .Invoke(null, new object[] { mediator, @params, token });
        }

        public static Task HandleRequest(IMediator mediator, IHandlerDescriptor handler, object @params, CancellationToken token)
        {
            if (handler.HandlerType.GetInterfaces().Any(x =>
                x.IsGenericType && typeof(IRequestHandler<>).IsAssignableFrom(x.GetGenericTypeDefinition())))
            {
                return (Task)SendRequestUnit
                    .MakeGenericMethod(handler.Params)
                    .Invoke(null, new object[] { mediator, @params, token });
            }
            else
            {
                return (Task)SendRequestResponse
                    .MakeGenericMethod(handler.Params, handler.Response)
                    .Invoke(null, new object[] { mediator, @params, token });
            }
        }

        private static Task SendRequest<T>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest
        {
            return mediator.Send(request, token);
        }

        private static Task<TResponse> SendRequest<T, TResponse>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest<TResponse>
        {
            return mediator.Send(request, token);
        }
    }
}
