using System;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer
{
    public static class HandlerTypeHelpers
    {
        private static readonly Type[] HandlerTypes = { typeof(INotificationHandler), typeof(INotificationHandler<>), typeof(IRequestHandler<>), typeof(IRequestHandler<,>), };

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
