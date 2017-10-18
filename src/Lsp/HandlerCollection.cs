using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer
{
    class HandlerCollection : IHandlerCollection
    {
        internal readonly HashSet<HandlerDescriptor> _handlers = new HashSet<HandlerDescriptor>();

        public IEnumerator<ILspHandlerDescriptor> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable Add(IEnumerable<IJsonRpcHandler> handlers)
        {
            return Add(handlers.ToArray());
        }

        public IDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var descriptors = new HashSet<HandlerDescriptor>();
            foreach (var handler in handlers)
            {
                foreach (var implementedInterface in handler.GetType().GetTypeInfo()
                    .ImplementedInterfaces
                    .Where(x => !string.IsNullOrWhiteSpace(LspHelper.GetMethodName(x))))
                {
                    var @interface = HandlerTypeHelpers.GetHandlerInterface(implementedInterface);
                    var registration = UnwrapGenericType(typeof(IRegistration<>), implementedInterface);
                    var capability = UnwrapGenericType(typeof(ICapability<>), implementedInterface);

                    Type @params = null;
                    if (@interface.GetTypeInfo().IsGenericType)
                    {
                        @params = @interface.GetTypeInfo().GetGenericArguments()[0];
                    }

                    var key = "default";
                    if (handler is IRegistration<TextDocumentRegistrationOptions>)
                    {
                        if (GetTextDocumentRegistrationOptionsMethod
                            .MakeGenericMethod(registration)
                            .Invoke(handler, new object[] { handler }) is TextDocumentRegistrationOptions options)
                            key = options.DocumentSelector;
                    }

                    var h = new HandlerDescriptor(
                        LspHelper.GetMethodName(implementedInterface),
                        key,
                        handler,
                        @interface,
                        @params,
                        registration,
                        capability,
                        () => _handlers.RemoveWhere(instance => instance.Handler == handler));

                    descriptors.Add(h);
                }
            }

            foreach (var handler in descriptors)
            {
                _handlers.Add(handler);
            }

            return new ImmutableDisposable(descriptors);
        }

        private static readonly MethodInfo GetTextDocumentRegistrationOptionsMethod = typeof(HandlerCollection).GetTypeInfo()
            .GetMethod(nameof(GetTextDocumentRegistrationOptions), BindingFlags.Static | BindingFlags.NonPublic);

        private static TextDocumentRegistrationOptions GetTextDocumentRegistrationOptions<T>(IRegistration<T> instance)
            where T : TextDocumentRegistrationOptions
        {
            return instance.GetRegistrationOptions();
        }

        private Type UnwrapGenericType(Type genericType, Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                ?.GetTypeInfo()
                ?.GetGenericArguments()[0];
        }
    }
}
