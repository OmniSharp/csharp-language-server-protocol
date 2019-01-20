using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class HandlerCollection : IHandlerCollection
    {
        private readonly ISupportedCapabilities _supportedCapabilities;
        private readonly HashSet<HandlerDescriptor> _textDocumentIdentifiers = new HashSet<HandlerDescriptor>();
        internal readonly HashSet<HandlerDescriptor> _handlers = new HashSet<HandlerDescriptor>();

        public HandlerCollection(ISupportedCapabilities supportedCapabilities)
        {
            _supportedCapabilities = supportedCapabilities;
        }

        public IEnumerable<ITextDocumentIdentifier> TextDocumentIdentifiers()
        {
            return _textDocumentIdentifiers
                .Select(descriptor => descriptor.Handler)
                .OfType<ITextDocumentIdentifier>();
        }

        public IEnumerator<ILspHandlerDescriptor> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public LspHandlerDescriptorDisposable Add(string method, IJsonRpcHandler handler)
        {
            var descriptor = GetDescriptor(method, handler.GetType(), handler);
            _handlers.Add(descriptor);
            return new LspHandlerDescriptorDisposable(descriptor);
        }

        public LspHandlerDescriptorDisposable Add(string method, IServiceProvider serviceProvider, Type handlerType)
        {
            var descriptor = GetDescriptor(method, handlerType, serviceProvider);
            _handlers.Add(descriptor);
            return new LspHandlerDescriptorDisposable(descriptor);
        }

        public LspHandlerDescriptorDisposable Add(IServiceProvider serviceProvider, params Type[] handlerTypes)
        {
            var descriptors = new HashSet<HandlerDescriptor>();
            foreach (var handlerType in handlerTypes)
            {
                foreach (var (method, implementedInterface) in handlerType.GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: LspHelper.GetMethodName(x), implementedInterface: x))
                    .Where(x => !string.IsNullOrWhiteSpace(x.method)))
                {
                    descriptors.Add(GetDescriptor(method, implementedInterface, serviceProvider));
                }
            }

            foreach (var descriptor in descriptors)
            {
                _handlers.Add(descriptor);
                if (typeof(ITextDocumentIdentifier).IsAssignableFrom(descriptor.ImplementationType))
                {
                    _textDocumentIdentifiers.Add(descriptor);
                }
            }

            return new LspHandlerDescriptorDisposable(descriptors);
        }

        public LspHandlerDescriptorDisposable Add(params IJsonRpcHandler[] handlers)
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
                if (descriptor.Handler is ITextDocumentIdentifier)
                {
                    _textDocumentIdentifiers.Add(descriptor);
                }
            }

            return new LspHandlerDescriptorDisposable(descriptors);
        }

        private HandlerDescriptor GetDescriptor(string method, Type handlerType, IServiceProvider serviceProvider)
        {
            return GetDescriptor(
                method,
                handlerType,
                ActivatorUtilities.CreateInstance(serviceProvider, handlerType) as IJsonRpcHandler);
        }

        private HandlerDescriptor GetDescriptor(string method, Type handlerType, IJsonRpcHandler handler)
        {
            var @interface = HandlerTypeHelpers.GetHandlerInterface(handlerType);
            var registrationType = UnwrapGenericType(typeof(IRegistration<>), handlerType);
            var capabilityType = UnwrapGenericType(typeof(ICapability<>), handlerType);

            Type @params = null;
            object registrationOptions = null;
            Registration registration = null;
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
            }

            if (registrationType != null)
            {
                registrationOptions = GetRegistrationMethod
                    .MakeGenericMethod(registrationType)
                    .Invoke(null, new object[] { handler });

                if (_supportedCapabilities.AllowsDynamicRegistration(capabilityType))
                {
                    registration = new Registration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Method = method,
                        RegisterOptions = registrationOptions
                    };
                }
            }

            var key = "default";
            if (handler is IRegistration<TextDocumentRegistrationOptions> handlerRegistration)
            {
                // Ensure we only do this check for the specific registartion type that was found
                if (typeof(TextDocumentRegistrationOptions).GetTypeInfo().IsAssignableFrom(registrationType))
                {
                    key = handlerRegistration?.GetRegistrationOptions()?.DocumentSelector ?? key;
                }
                // In some scenarios, users will implement both the main handler and the resolve handler to the same class
                // This allows us to get a key for those interfaces so we can register many resolve handlers
                // and then route those resolve requests to the correct handler
                if (handlerType.GetTypeInfo().ImplementedInterfaces.Any(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICanBeResolvedHandler<>)))
                {
                    key = handlerRegistration?.GetRegistrationOptions()?.DocumentSelector ?? key;
                }
            }

            if (string.IsNullOrWhiteSpace(key)) key = "default";

            var descriptor = new HandlerDescriptor(
                method,
                key,
                handler,
                @interface,
                @params,
                registrationType,
                registration,
                capabilityType,
                () =>
                {
                    _handlers.RemoveWhere(d => d.Handler == handler);
                    _textDocumentIdentifiers.RemoveWhere(d => d.Handler == handler);
                });

            LspHandlerDescriptorHelpers.InitializeHandler(descriptor, _supportedCapabilities, handler);

            return descriptor;
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
            return this.Any(z => z.HandlerType.GetTypeInfo().IsAssignableFrom(typeInfo) || z.ImplementationType.GetTypeInfo().IsAssignableFrom(typeInfo));
        }

        private static readonly MethodInfo GetRegistrationMethod = typeof(HandlerCollection)
            .GetTypeInfo()
            .GetMethod(nameof(GetRegistration), BindingFlags.NonPublic | BindingFlags.Static);

        private static object GetRegistration<T>(IRegistration<T> registration)
        {
            return registration.GetRegistrationOptions();
        }
    }
}
