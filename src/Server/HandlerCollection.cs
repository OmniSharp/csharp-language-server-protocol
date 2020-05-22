using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class HandlerCollection : IHandlerCollection
    {
        private static readonly MethodInfo GetRegistrationMethod = typeof(HandlerCollection)
            .GetTypeInfo()
            .GetMethod(nameof(GetRegistration), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly ISupportedCapabilities _supportedCapabilities;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        internal readonly HashSet<HandlerDescriptor> _handlers = new HashSet<HandlerDescriptor>();
        private IServiceProvider _serviceProvider;


        public HandlerCollection(ISupportedCapabilities supportedCapabilities,
            TextDocumentIdentifiers textDocumentIdentifiers)
        {
            _supportedCapabilities = supportedCapabilities;
            _textDocumentIdentifiers = textDocumentIdentifiers;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            var cd = new CompositeDisposable();
            if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }
            return new LspHandlerDescriptorDisposable(new[] { descriptor }, cd);
        }

        public LspHandlerDescriptorDisposable Add(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            var handler = handlerFunc(_serviceProvider);
            var descriptor = GetDescriptor(method, handler.GetType(), handler);
            _handlers.Add(descriptor);
            var cd = new CompositeDisposable();
            if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
            {
                _textDocumentIdentifiers.Add(textDocumentIdentifier);
            }
            return new LspHandlerDescriptorDisposable(new[] { descriptor }, cd);
        }

        public LspHandlerDescriptorDisposable Add(string method, Type handlerType)
        {
            var descriptor = GetDescriptor(method, handlerType, _serviceProvider);
            _handlers.Add(descriptor);
            var cd = new CompositeDisposable();
            if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }
            return new LspHandlerDescriptorDisposable(new [] {descriptor }, cd);
        }

        public LspHandlerDescriptorDisposable Add(params Type[] handlerTypes)
        {
            var descriptors = new HashSet<HandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handlerType in handlerTypes)
            {
                foreach (var (method, implementedInterface) in handlerType.GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: LspHelper.GetMethodName(x), implementedInterface: x))
                    .Where(x => !string.IsNullOrWhiteSpace(x.method)))
                {
                    var descriptor = GetDescriptor(method, implementedInterface, _serviceProvider);
                    descriptors.Add(descriptor);
                    _handlers.Add(descriptor);
                    if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
                    {
                        cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
                    }
                }
            }

            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        public LspHandlerDescriptorDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var descriptors = new HashSet<HandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: LspHelper.GetMethodName(x), implementedInterface: x))
                    .Where(x => !string.IsNullOrWhiteSpace(x.method)))
                {
                    var descriptor = GetDescriptor(method, implementedInterface, handler);
                    descriptors.Add(descriptor);
                    _handlers.Add(descriptor);
                    if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
                    {
                        cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
                    }
                }
            }

            return new LspHandlerDescriptorDisposable(descriptors, cd);
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
            if (@interface.GetTypeInfo().IsGenericType)
            {
                @params = @interface.GetTypeInfo().GetGenericArguments()[0];
            }

            if (registrationType != null)
            {
                registrationOptions = GetRegistrationMethod
                    .MakeGenericMethod(registrationType)
                    .Invoke(null, new object[] { handler });
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
                registrationOptions,
                registrationType != null && _supportedCapabilities.AllowsDynamicRegistration(capabilityType),
                capabilityType,
                () => {
                    _handlers.RemoveWhere(d => d.Handler == handler);
                });

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

        private static object GetRegistration<T>(IRegistration<T> registration)
            where T : class, new()
        {
            return registration.GetRegistrationOptions() ?? new T();
        }
    }
}
