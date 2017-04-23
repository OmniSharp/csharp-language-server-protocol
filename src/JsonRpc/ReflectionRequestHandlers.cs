using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace JsonRpc
{
    public static class ReflectionRequestHandlers
    {
        public static Task HandleNotification(IHandlerInstance instance)
        {
            var method = instance.HandlerType
                .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new object[0]);
        }

        public static Task HandleNotification(IHandlerInstance instance, object @params)
        {
            var method = instance.HandlerType
                .GetMethod(nameof(INotificationHandler.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new[] { @params });
        }

        public static Task HandleRequest(IHandlerInstance instance, CancellationToken token)
        {
            var method = instance.HandlerType
                .GetMethod(nameof(IRequestHandler<object>.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new object[] { token });
        }

        public static Task HandleRequest(IHandlerInstance instance, object @params, CancellationToken token)
        {
            var method = instance.HandlerType
                .GetMethod(nameof(IRequestHandler<object, object>.Handle), BindingFlags.Public | BindingFlags.Instance);

            return (Task)method.Invoke(instance.Handler, new[] { @params, token });
        }
    }
}