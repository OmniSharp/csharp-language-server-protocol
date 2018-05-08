using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class MediatRHandlers
    {
        private static readonly MethodInfo PublishNotificationMethod = typeof(MediatRHandlers)
            .GetMethod(nameof(PublishNotification), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo SendRequestMethod = typeof(MediatRHandlers)
            .GetMethod(nameof(SendRequest), BindingFlags.NonPublic | BindingFlags.Static);

        public static Task HandleNotification(IMediator mediator, IHandlerDescriptor handler, object @params, CancellationToken token)
        {
            return (Task)PublishNotificationMethod
                .MakeGenericMethod(handler.Params ?? typeof(INotification))
                .Invoke(null, new object[] { mediator, @params, token });
        }

        private static Task PublishNotification<T>(IMediator mediator, T notification, CancellationToken token)
            where T : INotification
        {
            return mediator.Publish(notification, token);
        }

        public static Task HandleRequest(IMediator mediator, IHandlerDescriptor handler, object @params, CancellationToken token)
        {
            return (Task)SendRequestMethod
                .MakeGenericMethod(handler.Params ?? typeof(INotification))
                .Invoke(null, new object[] { mediator, @params, token });
        }

        private static Task SendRequest<T>(IMediator mediator, T request, CancellationToken token)
            where T : IRequest
        {
            return mediator.Send(request, token);
        }
    }
}
