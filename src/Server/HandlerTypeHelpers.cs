using System;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class HandlerTypeHelpers
    {
        private static readonly Type[] HandlerTypes = { typeof(IJsonRpcNotificationHandler), typeof(IJsonRpcNotificationHandler<>), typeof(IJsonRpcRequestHandler<>), typeof(IJsonRpcRequestHandler<,>), };

        private static bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return HandlerTypes.Contains(type.GetGenericTypeDefinition());
            }
            return HandlerTypes.Contains(type);
        }

        public static Type GetHandlerInterface(Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }
    }
}
