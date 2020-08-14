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

        private static readonly ILookup<string, ILspHandlerTypeDescriptor> KnownHandlers;

        static LspHandlerTypeDescriptorHelper()
        {
            try
            {
                KnownHandlers = HandlerTypeDescriptorHelper.KnownHandlers.SelectMany(x => x)
                                                           .Select(x => new LspHandlerTypeDescriptor(x.HandlerType) as ILspHandlerTypeDescriptor)
                                                           .ToLookup(x => x.Method, x => x, StringComparer.Ordinal);
            }
            catch (Exception e)
            {
                throw new AggregateException("Failed", e);
            }
        }

        public static string GetMethodForRegistrationOptions(object registrationOptions)
        {
            var registrationType = registrationOptions.GetType();
            var interfaces = new HashSet<Type>(
                registrationOptions.GetType().GetInterfaces()
                                   .Except(registrationType.BaseType?.GetInterfaces() ?? Enumerable.Empty<Type>())
            );
            return interfaces.SelectMany(
                                  x =>
                                      KnownHandlers.SelectMany(z => z)
                                                   .Where(z => z.HasRegistration)
                                                   .Where(z => x.IsAssignableFrom(z.RegistrationType))
                              )
                             .FirstOrDefault()?.Method;
        }

        public static Type GetRegistrationType(string method) => KnownHandlers[method]
                                                                .Where(z => z.HasRegistration)
                                                                .Select(z => z.RegistrationType)
                                                                .FirstOrDefault();
        public static ILspHandlerTypeDescriptor GetHandlerTypeDescriptor<T>() => GetHandlerTypeDescriptor(typeof(T));

        public static ILspHandlerTypeDescriptor GetHandlerTypeDescriptor(Type type)
        {
            var @default = KnownHandlers
                          .SelectMany(g => g)
                          .FirstOrDefault(x => x.InterfaceType == type || x.HandlerType == type || x.ParamsType == type)
                ?? KnownHandlers
                  .SelectMany(g => g)
                  .FirstOrDefault(x => x.InterfaceType.IsAssignableFrom(type) || x.HandlerType.IsAssignableFrom(type));
            if (@default != null)
            {
                return @default;
            }

            var methodName = HandlerTypeDescriptorHelper.GetMethodName(type);
            return string.IsNullOrWhiteSpace(methodName) ? null : KnownHandlers[methodName].FirstOrDefault();
        }
    }
}
