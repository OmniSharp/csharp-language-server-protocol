using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
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
        private ImmutableHashSet<LspHandlerDescriptor> _descriptors = ImmutableHashSet<LspHandlerDescriptor>.Empty;
        private readonly IServiceProvider _serviceProvider;

        public SharedHandlerCollection(ISupportedCapabilities supportedCapabilities, TextDocumentIdentifiers textDocumentIdentifiers, IFallbackServiceProvider serviceProvider)
        {
            _supportedCapabilities = supportedCapabilities;
            _textDocumentIdentifiers = textDocumentIdentifiers;
            _serviceProvider = serviceProvider;
        }

        public IEnumerator<ILspHandlerDescriptor> GetEnumerator()
        {
            return _descriptors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IDisposable IHandlersManager.Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options) => Add(handler, options);

        IDisposable IHandlersManager.Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => Add(method, handler, options);

        IDisposable IHandlersManager.Add(JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options) => Add(handlerFactory(_serviceProvider), options);

        IDisposable IHandlersManager.Add(string method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options) => Add(method, handlerFactory(_serviceProvider), options);

        IDisposable IHandlersManager.Add(Type handlerType, JsonRpcHandlerOptions options) =>  Add(ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, options);

        IDisposable IHandlersManager.Add(string method, Type handlerType, JsonRpcHandlerOptions options) =>  Add(method, ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler, options);

        IDisposable IHandlersManager.AddLink(string sourceMethod, string destinationMethod)
        {
            var source = _descriptors.First(z => z.Method == sourceMethod);
            LspHandlerDescriptor descriptor = null;
            descriptor = GetDescriptor(
                destinationMethod,
                source.HandlerType,
                source.Handler,
                source.RequestProcessType.HasValue ? new JsonRpcHandlerOptions() {RequestProcessType = source.RequestProcessType.Value} : null,
                source.TypeDescriptor,
                source.HandlerType,
                source.RegistrationType,
                source.CapabilityType);
            Interlocked.Exchange(ref _descriptors, _descriptors.Add(descriptor));
            var cd = new CompositeDisposable();
            if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }

            return new LspHandlerDescriptorDisposable(new[] {descriptor}, cd);
        }


        public LspHandlerDescriptorDisposable Add(params IJsonRpcHandler[] handlers)
        {
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handler in handlers)
            {
                var (innerDescriptors, innerCompositeDisposable ) = AddHandler(handler, null);
                innerDescriptors.UnionWith(descriptors);
                cd.Add(innerCompositeDisposable);
            }
            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        public LspHandlerDescriptorDisposable Add(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var (descriptors, compositeDisposable ) = AddHandler(handler, options);
            return new LspHandlerDescriptorDisposable(descriptors, compositeDisposable);
        }

        public LspHandlerDescriptorDisposable Add(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var (descriptors, cd) = AddHandler(method, handler, options);
            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        public LspHandlerDescriptorDisposable Add(params JsonRpcHandlerFactory[] handlerFactories)
        {
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handlerFactory in handlerFactories)
            {
                var (innerDescriptors, innerCompositeDisposable ) = AddHandler(handlerFactory(_serviceProvider), null);
                innerDescriptors.UnionWith(descriptors);
                cd.Add(innerCompositeDisposable);
            }
            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        public LspHandlerDescriptorDisposable Add(JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options)
        {
            var (descriptors, compositeDisposable ) = AddHandler(handlerFactory(_serviceProvider), null);
            return new LspHandlerDescriptorDisposable(descriptors, compositeDisposable);
        }

        public LspHandlerDescriptorDisposable Add(string method, JsonRpcHandlerFactory handlerFactory, JsonRpcHandlerOptions options)
        {
            var (descriptors, cd) = AddHandler(method, handlerFactory(_serviceProvider), options);
            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        public LspHandlerDescriptorDisposable Add(params Type[] handlerTypes)
        {
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var handlerType in handlerTypes)
            {
                var (innerDescriptors, innerCompositeDisposable ) = AddHandler(ActivatorUtilities.CreateInstance(_serviceProvider, handlerType)as IJsonRpcHandler , null);
                innerDescriptors.UnionWith(descriptors);
                cd.Add(innerCompositeDisposable);
            }
            return new LspHandlerDescriptorDisposable(descriptors, cd);

        }

        public LspHandlerDescriptorDisposable Add(Type handlerType, JsonRpcHandlerOptions options)
        {
            var (descriptors, cd) = AddHandler(ActivatorUtilities.CreateInstance(_serviceProvider, handlerType)as IJsonRpcHandler, options);
            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        public LspHandlerDescriptorDisposable Add(string method, Type handlerType, JsonRpcHandlerOptions options)
        {
            var (descriptors, cd) = AddHandler(method, ActivatorUtilities.CreateInstance(_serviceProvider, handlerType)as IJsonRpcHandler, options);
            return new LspHandlerDescriptorDisposable(descriptors, cd);
        }

        class EqualityComparer : IEqualityComparer<(string method, Type implementedInterface)>
        {
            public bool Equals((string method, Type implementedInterface) x, (string method, Type implementedInterface) y)
            {
                return x.method?.Equals(y.method) == true;
            }

            public int GetHashCode((string method, Type implementedInterface) obj)
            {
                return obj.method?.GetHashCode() ?? 0;
            }
        }

        public (HashSet<LspHandlerDescriptor> descriptors, CompositeDisposable compositeDisposable) AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            var descriptor = GetDescriptor(method, handler.GetType(), handler, options);
            descriptors.Add(descriptor);
            Interlocked.Exchange(ref _descriptors, _descriptors.Add(descriptor));
            if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }

            return (descriptors, cd);
        }

        private (HashSet<LspHandlerDescriptor> descriptors, CompositeDisposable compositeDisposable) AddHandler(IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var descriptors = new HashSet<LspHandlerDescriptor>();
            var cd = new CompositeDisposable();
            foreach (var (method, implementedInterface) in handler.GetType().GetTypeInfo()
                .ImplementedInterfaces
                .Select(x => (method: HandlerTypeDescriptorHelper.GetMethodName(x), implementedInterface: x))
                .Distinct(new EqualityComparer())
                .Where(x => !string.IsNullOrWhiteSpace(x.method))
            )
            {
                var descriptor = GetDescriptor(method, implementedInterface, handler, options);
                descriptors.Add(descriptor);
                Interlocked.Exchange(ref _descriptors, _descriptors.Add(descriptor));
                if (descriptor.Handler is ITextDocumentIdentifier textDocumentIdentifier)
                {
                    cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
                }
            }

            return (descriptors, cd);
        }

        private LspHandlerDescriptor GetDescriptor(string method, Type handlerType, IServiceProvider serviceProvider, JsonRpcHandlerOptions options)
        {
            return GetDescriptor(
                method,
                handlerType,
                ActivatorUtilities.CreateInstance(serviceProvider, handlerType) as IJsonRpcHandler,
                options);
        }

        private LspHandlerDescriptor GetDescriptor(string method, Type handlerType, IJsonRpcHandler handler, JsonRpcHandlerOptions options)
        {
            var typeDescriptor = LspHandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(method);
            var @interface = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);
            var registrationType = typeDescriptor?.RegistrationType ??
                                   HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(IRegistration<>), handlerType);
            var capabilityType = typeDescriptor?.CapabilityType ??
                                 HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(ICapability<>), handlerType);

            return GetDescriptor(method, handlerType, handler, options, typeDescriptor, @interface, registrationType, capabilityType);
        }

        private LspHandlerDescriptor GetDescriptor(string method, Type handlerType, IJsonRpcHandler handler, JsonRpcHandlerOptions options,
            ILspHandlerTypeDescriptor typeDescriptor,
            Type @interface, Type registrationType, Type capabilityType)
        {
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
                    .Invoke(null, new object[] {handler});
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
                if (handler.GetType().GetTypeInfo().ImplementedInterfaces
                    .Any(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICanBeResolvedHandler<>)))
                {
                    key = handlerRegistration?.GetRegistrationOptions()?.DocumentSelector ?? key;
                }
            }
            else if (handler is IRegistration<ExecuteCommandRegistrationOptions> commandRegistration)
            {
                key = string.Join("|", commandRegistration.GetRegistrationOptions()?.Commands ?? Array.Empty<string>());
            }

            if (string.IsNullOrWhiteSpace(key)) key = "default";

            if (handler is ICanBeIdentifiedHandler identifiedHandler && identifiedHandler.Id != Guid.Empty)
            {
                key += ":" + identifiedHandler.Id.ToString("N");
            }

            var requestProcessType =
                options?.RequestProcessType ??
                typeDescriptor?.RequestProcessType ??
                handlerType.GetCustomAttributes(true)
                    .Concat(@interface.GetCustomAttributes(true))
                    .OfType<ProcessAttribute>()
                    .FirstOrDefault()?.Type;

            var descriptor = new LspHandlerDescriptor(
                method,
                key,
                handler,
                @interface,
                @params,
                registrationType,
                registrationOptions,
                (registrationType == null ? (Func<bool>) (() => false) : (() => _supportedCapabilities.AllowsDynamicRegistration(capabilityType))),
                capabilityType,
                requestProcessType,
                () => {
                    var descriptors = _descriptors.ToBuilder();
                    foreach (var descriptor in _descriptors)
                    {
                        if (descriptor.Handler != handler) continue;
                        descriptors.Remove(descriptor);
                    }

                    Interlocked.Exchange(ref _descriptors, descriptors.ToImmutable());
                },
                typeDescriptor);

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
