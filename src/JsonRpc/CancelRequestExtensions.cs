using System;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class CancelRequestExtensions
    {
        public static IJsonRpcHandlerRegistry<IJsonRpcServerRegistry> OnCancelRequest(
            this IJsonRpcHandlerRegistry<IJsonRpcServerRegistry> registry,
            Action<CancelParams> handler)
        {
            return registry.AddHandler(JsonRpcNames.CancelRequest, NotificationHandler.For(handler));
        }

        public static IJsonRpcHandlerRegistry<IJsonRpcServerRegistry> OnCancelRequest(
            this IJsonRpcHandlerRegistry<IJsonRpcServerRegistry> registry,
            Func<CancelParams, Task> handler)
        {
            return registry.AddHandler(JsonRpcNames.CancelRequest, NotificationHandler.For(handler));
        }
    }
}