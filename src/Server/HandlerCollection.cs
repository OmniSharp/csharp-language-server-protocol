using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class HandlerCollection : IHandlerCollection
    {
        internal readonly HashSet<HandlerDescriptor> _handlers = new HashSet<HandlerDescriptor>();
        internal readonly HashSet<ITextDocumentSyncHandler> _documentSyncHandlers = new HashSet<ITextDocumentSyncHandler>();

        public IEnumerable<ITextDocumentSyncHandler> TextDocumentSyncHandlers()
        {
            return _documentSyncHandlers;
        }

        public IEnumerator<ILspHandlerDescriptor> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable Add(string method, IJsonRpcHandler handler)
        {
            var descriptor = GetDescriptor(method, handler.GetType(), handler);
            _handlers.Add(descriptor);
            return descriptor;
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
                foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: LspHelper.GetMethodName(x), implementedInterface: x))
                    .Where(x => !string.IsNullOrWhiteSpace(x.method)))
                {
                    descriptors.Add(GetDescriptor(method, implementedInterface, handler));
                }
            }

            foreach (var descriptor in descriptors)
            {
                _handlers.Add(descriptor);
                if (descriptor.Handler is ITextDocumentSyncHandler documentSyncHandler)
                {
                    _documentSyncHandlers.Add(documentSyncHandler);
                }
            }

            return new ImmutableDisposable(descriptors);
        }

        private HandlerDescriptor GetDescriptor(string method, Type implementedType, IJsonRpcHandler handler)
        {
            var @interface = HandlerTypeHelpers.GetHandlerInterface(implementedType);
            var registration = UnwrapGenericType(typeof(IRegistration<>), implementedType);
            var capability = UnwrapGenericType(typeof(ICapability<>), implementedType);

            Type @params = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
            }

            var key = "default";
            // This protects against the case where class implements many, possibly conflicting, interfaces.
            if ((registration != null && typeof(TextDocumentRegistrationOptions).GetTypeInfo().IsAssignableFrom(registration) ||
                registration == null && implementedType.GetTypeInfo().ImplementedInterfaces.Any(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICanBeResolvedHandler<>))) &&
                handler is IRegistration<TextDocumentRegistrationOptions> handlerRegistration)
            {
                key = string.IsNullOrEmpty(handlerRegistration?.GetRegistrationOptions()?.DocumentSelector)
                    ? key
                    : handlerRegistration?.GetRegistrationOptions()?.DocumentSelector;
            }

            return new HandlerDescriptor(
                method,
                key,
                handler,
                @interface,
                @params,
                registration,
                capability,
                () => {
                    _handlers.RemoveWhere(instance => instance.Handler == handler);
                    _documentSyncHandlers.RemoveWhere(instance => instance == handler);
                });
        }

        private Type UnwrapGenericType(Type genericType, Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                ?.GetTypeInfo()
                ?.GetGenericArguments()[0];
        }

        public bool ContainsHandler(Type type)
        {
            return ContainsHandler(type.GetTypeInfo());
        }

        public bool ContainsHandler(TypeInfo typeInfo)
        {
            return this.Any(z =>
                    z.HandlerType.GetTypeInfo().IsAssignableFrom(typeInfo) ||
                    z.Handler.GetType().GetTypeInfo().IsAssignableFrom(typeInfo));
        }
    }
}
