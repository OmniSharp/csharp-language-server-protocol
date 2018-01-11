using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class ReflectionRequestHandlers
    {
        public static Task HandleNotification(IHandlerDescriptor instance)
        {
            var method = instance.HandlerType.GetTypeInfo()
                .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new object[0]);
        }

        public static Task HandleNotification(IHandlerDescriptor instance, object @params)
        {
            var method = instance.HandlerType.GetTypeInfo()
                .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new[] { @params });
        }

        public static Task HandleRequest(IHandlerDescriptor instance, object @params, CancellationToken token)
        {
            var method = instance.HandlerType.GetTypeInfo()
                .GetMethod(nameof(IRequestHandler<object, object>.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new[] { @params, token });
        }
    }
}
