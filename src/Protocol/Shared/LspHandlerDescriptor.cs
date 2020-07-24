using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    [DebuggerDisplay("{Key}:{Method}")]
    class LspHandlerDescriptor : ILspHandlerDescriptor, IDisposable, IEquatable<LspHandlerDescriptor>
    {
        private readonly Action _disposeAction;
        private readonly Func<bool> _allowsDynamicRegistration;

        public LspHandlerDescriptor(
            string method,
            string key,
            IJsonRpcHandler handler,
            Type handlerType,
            Type @params,
            Type registrationType,
            object registrationOptions,
            Func<bool> allowsDynamicRegistration,
            Type capabilityType,
            RequestProcessType? requestProcessType,
            Action disposeAction,
            ILspHandlerTypeDescriptor typeDescriptor)
        {
            _disposeAction = disposeAction;
            Id = Guid.NewGuid();
            Method = method;
            Key = key;
            ImplementationType = handler.GetType();
            Handler = handler;
            HandlerType = handlerType;
            Params = @params;
            RegistrationType = registrationType;
            RegistrationOptions = registrationOptions;
            _allowsDynamicRegistration = allowsDynamicRegistration;
            CapabilityType = capabilityType;

            var requestInterface = @params?.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                Response = requestInterface.GetGenericArguments()[0];

            // If multiple are implemented this behavior is unknown
            CanBeResolvedHandlerType = handler.GetType().GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICanBeResolvedHandler<>));

            HasReturnType = HandlerType.GetInterfaces().Any(@interface =>
                @interface.IsGenericType &&
                typeof(IRequestHandler<,>).IsAssignableFrom(@interface.GetGenericTypeDefinition())
            );

            IsDelegatingHandler = @params?.IsGenericType == true &&
                                  (
                                      typeof(DelegatingRequest<>).IsAssignableFrom(@params.GetGenericTypeDefinition()) ||
                                      typeof(DelegatingNotification<>).IsAssignableFrom(@params.GetGenericTypeDefinition())
                                  );
            if (handler is IOnServerStarted serverStarted)
            {
                OnServerStartedDelegate = serverStarted.OnStarted;
            }
            if (handler is IOnClientStarted clientStarted)
            {
                OnClientStartedDelegate = clientStarted.OnStarted;
            }

            IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerType) || handlerType
                .GetInterfaces().Any(z =>
                    z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z.GetGenericTypeDefinition()));
            IsRequest = !IsNotification;
            RequestProcessType = requestProcessType;
            TypeDescriptor = typeDescriptor;
        }

        public Type ImplementationType { get; }
        public Type HandlerType { get; }

        public Guid Id { get; }
        public bool HasRegistration => RegistrationType != null;
        public Type RegistrationType { get; }
        public object RegistrationOptions { get; }
        public bool AllowsDynamicRegistration => _allowsDynamicRegistration();

        public bool HasCapability => CapabilityType != null;
        public Type CapabilityType { get; }
        public OnServerStartedDelegate OnServerStartedDelegate { get; }
        public OnClientStartedDelegate OnClientStartedDelegate { get; }

        public string Method { get; }
        public string Key { get; }
        public Type Params { get; }
        public Type Response { get; }
        public bool IsDelegatingHandler { get; }

        public bool IsDynamicCapability => typeof(IDynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
        public Type CanBeResolvedHandlerType { get; }
        public bool HasReturnType { get; }

        public IJsonRpcHandler Handler { get; }
        public bool IsNotification { get; }
        public bool IsRequest { get; }
        public RequestProcessType? RequestProcessType { get; }
        public ILspHandlerTypeDescriptor TypeDescriptor { get; }

        public void Dispose()
        {
            _disposeAction();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LspHandlerDescriptor);
        }

        public bool Equals(LspHandlerDescriptor other)
        {
            return other != null &&
                   EqualityComparer<Type>.Default.Equals(HandlerType, other.HandlerType) &&
                   Method == other.Method &&
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            var hashCode = -45133801;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(HandlerType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Method);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
            return hashCode;
        }

        public static bool operator ==(LspHandlerDescriptor descriptor1, LspHandlerDescriptor descriptor2)
        {
            return EqualityComparer<LspHandlerDescriptor>.Default.Equals(descriptor1, descriptor2);
        }

        public static bool operator !=(LspHandlerDescriptor descriptor1, LspHandlerDescriptor descriptor2)
        {
            return !(descriptor1 == descriptor2);
        }
    }
}
