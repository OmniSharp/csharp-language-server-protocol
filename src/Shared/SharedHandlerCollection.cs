using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    class SharedHandlerCollection : IHandlerCollection
    {
        private static readonly MethodInfo GetRegistrationMethod = typeof(SharedHandlerCollection)
            .GetTypeInfo()
            .GetMethod(nameof(GetRegistration), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly ISupportedCapabilities _supportedCapabilities;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        internal readonly HashSet<LspHandlerDescriptor> _handlers = new HashSet<LspHandlerDescriptor>();
        private IServiceProvider _serviceProvider;

        public SharedHandlerCollection(ISupportedCapabilities supportedCapabilities,
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

        IDisposable IHandlersManager.Add(IJsonRpcHandler handler) => Add(handler);

        IDisposable IHandlersManager.Add(string method, IJsonRpcHandler handler) => Add(method, handler);

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
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
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
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handlerType in handlerTypes)
            {
                foreach (var (method, implementedInterface) in handlerType.GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: HandlerTypeDescriptorHelper.GetMethodName(x), implementedInterface: x))
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
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                if (descriptors.Any(z => z.Handler == handler)) continue;

                foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(x => (method: HandlerTypeDescriptorHelper.GetMethodName(x), implementedInterface: x))
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

        private LspHandlerDescriptor GetDescriptor(string method, Type handlerType, IServiceProvider serviceProvider)
        {
            return GetDescriptor(
                method,
                handlerType,
                ActivatorUtilities.CreateInstance(serviceProvider, handlerType) as IJsonRpcHandler);
        }

        private LspHandlerDescriptor GetDescriptor(string method, Type handlerType, IJsonRpcHandler handler)
        {
            var typeDescriptor = LspHandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(method);
            var @interface = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);
            var registrationType = typeDescriptor?.RegistrationType ?? HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(IRegistration<>), handlerType);
            var capabilityType = typeDescriptor?.CapabilityType ?? HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(ICapability<>), handlerType);

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
                if (handler.GetType().GetTypeInfo().ImplementedInterfaces.Any(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICanBeResolvedHandler<>)))
                {
                    key = handlerRegistration?.GetRegistrationOptions()?.DocumentSelector ?? key;
                }
            }

            if (string.IsNullOrWhiteSpace(key)) key = "default";

            var descriptor = new LspHandlerDescriptor(
                method,
                key,
                handler,
                @interface,
                @params,
                registrationType,
                registrationOptions,
                (registrationType ==  null ? (Func<bool>) (() => false) : (() => _supportedCapabilities.AllowsDynamicRegistration(capabilityType))),
                capabilityType,
                () => {
                    _handlers.RemoveWhere(d => d.Handler == handler);
                });

            return descriptor;
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
