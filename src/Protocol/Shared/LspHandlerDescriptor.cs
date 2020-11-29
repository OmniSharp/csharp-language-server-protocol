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
    internal class LspHandlerDescriptor : ILspHandlerDescriptor, IDisposable, IEquatable<LspHandlerDescriptor>
    {
        public int Index { get; }
        private readonly Action _disposeAction;

        public LspHandlerDescriptor(
            int index,
            string method,
            string key,
            IJsonRpcHandler handler,
            Type handlerType,
            Type? @params,
            Type? registrationType,
            object? registrationOptions,
            Type? capabilityType,
            RequestProcessType? requestProcessType,
            Action disposeAction,
            ILspHandlerTypeDescriptor? typeDescriptor
        ) : this(
            index,
            method, key, handler, handlerType, @params, registrationType, registrationOptions, capabilityType, requestProcessType, disposeAction,
            typeDescriptor, null
        )
        {
        }

        public LspHandlerDescriptor(
            int index,
            string method,
            string key,
            IJsonRpcHandler handler,
            Type handlerType,
            Type? @params,
            Type? registrationType,
            object? registrationOptions,
            Type? capabilityType,
            RequestProcessType? requestProcessType,
            Action disposeAction,
            ILspHandlerTypeDescriptor? typeDescriptor,
            Guid? id
        )
        {
            _disposeAction = disposeAction;
            Id = id.HasValue ? id.Value : handler is ICanBeIdentifiedHandler h && h.Id != Guid.Empty ? h.Id : Guid.NewGuid();
            Method = method;
            Key = key;
            ImplementationType = handler.GetType();
            Handler = handler;
            HandlerType = handlerType;
            Params = @params;
            RegistrationType = registrationType;
            RegistrationOptions = registrationOptions;
            CapabilityType = capabilityType;

            Response = typeDescriptor?.ResponseType ??
                       @params?.GetInterfaces()
                               .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                              ?.GetGenericArguments()[0] ?? typeof(Unit);

            // If multiple are implemented this behavior is unknown
            CanBeResolvedHandlerType = handler.GetType().GetTypeInfo()
                                             ?.ImplementedInterfaces
                                             ?.FirstOrDefault(x => typeof(ICanBeResolvedHandler).IsAssignableFrom(x));

            HasReturnType = Response != null && Response != typeof(Unit);

            IsDelegatingHandler = @params?.IsGenericType == true &&
                                  (
                                      typeof(DelegatingRequest<>).IsAssignableFrom(@params.GetGenericTypeDefinition()) ||
                                      typeof(DelegatingNotification<>).IsAssignableFrom(@params.GetGenericTypeDefinition())
                                  );

            IsNotification = handlerType.GetInterfaces().Any(z => z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z.GetGenericTypeDefinition()));
            IsRequest = !IsNotification;
            RequestProcessType = requestProcessType;
            TypeDescriptor = typeDescriptor;
            Index = index;
            IsBuiltIn = handler.GetType().GetCustomAttributes<BuiltInAttribute>().Any();
        }

        public LspHandlerDescriptor(
            LspHandlerDescriptor descriptor,
            string key,
            object? registrationOptions
        ) : this(descriptor.Index, descriptor.Method, key, descriptor.Handler, descriptor.HandlerType, descriptor.Params, descriptor.RegistrationType, registrationOptions, descriptor.CapabilityType, descriptor.RequestProcessType, descriptor._disposeAction, descriptor.TypeDescriptor, descriptor.Id)
        {
        }

        public Type ImplementationType { get; }
        public Type HandlerType { get; }

        public bool IsBuiltIn { get; set; }

        public Guid Id { get; }
        public bool HasRegistration => RegistrationType != null;
        public Type? RegistrationType { get; }
        public object? RegistrationOptions { get; }

        public bool HasCapability => CapabilityType != null;
        public Type? CapabilityType { get; }

        public string Method { get; }
        public string Key { get; }
        public Type? Params { get; }
        public Type? Response { get; }
        public bool IsDelegatingHandler { get; }

        public bool IsDynamicCapability => typeof(IDynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
        public Type? CanBeResolvedHandlerType { get; }
        public bool HasReturnType { get; }

        public IJsonRpcHandler Handler { get; }
        public bool IsNotification { get; }
        public bool IsRequest { get; }
        public RequestProcessType? RequestProcessType { get; }
        public ILspHandlerTypeDescriptor? TypeDescriptor { get; }

        public void Dispose() => _disposeAction();

        public override bool Equals(object? obj) => Equals(obj as LspHandlerDescriptor);

        public bool Equals(LspHandlerDescriptor? other) =>
            other is not null &&
//            EqualityComparer<Type>.Default.Equals(HandlerType, other.HandlerType) &&
            Method == other.Method &&
            Key == other.Key;

        public override int GetHashCode()
        {
            var hashCode = -45133801;
//            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(HandlerType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Method);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
            return hashCode;
        }

        public static bool operator ==(LspHandlerDescriptor descriptor1, LspHandlerDescriptor descriptor2) =>
            EqualityComparer<LspHandlerDescriptor>.Default.Equals(descriptor1, descriptor2);

        public static bool operator !=(LspHandlerDescriptor descriptor1, LspHandlerDescriptor descriptor2) => !( descriptor1 == descriptor2 );

        internal class AllowAllEqualityComparer : IEqualityComparer<LspHandlerDescriptor>
        {
            public bool Equals(LspHandlerDescriptor x, LspHandlerDescriptor y) => x.Id == y.Id;

            public int GetHashCode(LspHandlerDescriptor obj) => obj.Id.GetHashCode();
        }
    }
}
