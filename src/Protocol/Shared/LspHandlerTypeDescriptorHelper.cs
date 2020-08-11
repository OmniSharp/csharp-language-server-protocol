using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public static class LspHandlerTypeDescriptorHelper
    {
        private static readonly ConcurrentDictionary<Type, string> MethodNames =
            new ConcurrentDictionary<Type, string>();

        private static readonly ImmutableSortedDictionary<string, ILspHandlerTypeDescriptor> KnownHandlers;

        static LspHandlerTypeDescriptorHelper()
        {
            try
            {
                KnownHandlers = HandlerTypeDescriptorHelper.KnownHandlers.Values
                                                           .Select(x => new LspHandlerTypeDescriptor(x.HandlerType) as ILspHandlerTypeDescriptor)
                                                           .ToImmutableSortedDictionary(x => x.Method, x => x, StringComparer.Ordinal);
            }
            catch (Exception e)
            {
                throw new AggregateException("Failed", e);
            }
        }

        public static ILspHandlerTypeDescriptor GetHandlerTypeForRegistrationOptions(object registrationOptions)
        {
            var registrationType = registrationOptions.GetType();
            var interfaces = new HashSet<Type>(
                registrationOptions.GetType().GetInterfaces()
                                   .Except(registrationType.BaseType?.GetInterfaces() ?? Enumerable.Empty<Type>())
            );
            return interfaces.SelectMany(
                                  x =>
                                      KnownHandlers.Values
                                                   .Where(z => z.HasRegistration)
                                                   .Where(z => x.IsAssignableFrom(z.RegistrationType))
                              )
                             .FirstOrDefault();
        }

        public static ILspHandlerTypeDescriptor GetHandlerTypeDescriptor(string method) => KnownHandlers.TryGetValue(method, out var descriptor) ? descriptor : null;

        public static ILspHandlerTypeDescriptor GetHandlerTypeDescriptor<T>() =>
            KnownHandlers.Values.FirstOrDefault(x => x.InterfaceType == typeof(T)) ??
            GetHandlerTypeDescriptor(HandlerTypeDescriptorHelper.GetMethodName(typeof(T)));

        public static ILspHandlerTypeDescriptor GetHandlerTypeDescriptor(Type type)
        {
            var @default = KnownHandlers.Values.FirstOrDefault(x => x.InterfaceType == type);
            if (@default != null)
            {
                return @default;
            }

            var methodName = HandlerTypeDescriptorHelper.GetMethodName(type);
            if (string.IsNullOrWhiteSpace(methodName)) return null;
            return GetHandlerTypeDescriptor(methodName);
        }
    }
}
