using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class HandlerDescriptor : ILspHandlerDescriptor, IDisposable, IEquatable<HandlerDescriptor>
    {
        private readonly Action _disposeAction;

        public HandlerDescriptor(
            string method,
            string key,
            IJsonRpcHandler handler,
            Type handlerType,
            Type @params,
            Type registrationType,
            Registration registration,
            Type capabilityType,
            Action disposeAction)
        {
            _disposeAction = disposeAction;
            Method = method;
            Key = key;
            ImplementationType = handler.GetType();
            Handler = handler;
            HandlerType = handlerType;
            Params = @params;
            Response = Response;
            RegistrationType = registrationType;
            Registration = registration;
            CapabilityType = capabilityType;

            var requestInterface = @params?.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                Response = requestInterface.GetGenericArguments()[0];

            // If multiple are implemented this behavior is unknown
            CanBeResolvedHandlerType = handler.GetType().GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICanBeResolvedHandler<>));
        }

        public Type ImplementationType { get; }
        public Type HandlerType { get; }

        public bool HasRegistration => RegistrationType != null;
        public Type RegistrationType { get; }
        public Registration Registration { get; }

        public bool HasCapability => CapabilityType != null;
        public Type CapabilityType { get; }

        public string Method { get; }
        public string Key { get; }
        public Type Params { get; }
        public Type Response { get; }

        public bool IsDynamicCapability => typeof(DynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
        public Type CanBeResolvedHandlerType { get; }

        public IJsonRpcHandler Handler { get; }

        public void Dispose()
        {
            _disposeAction();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HandlerDescriptor);
        }

        public bool Equals(HandlerDescriptor other)
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

        public static bool operator ==(HandlerDescriptor descriptor1, HandlerDescriptor descriptor2)
        {
            return EqualityComparer<HandlerDescriptor>.Default.Equals(descriptor1, descriptor2);
        }

        public static bool operator !=(HandlerDescriptor descriptor1, HandlerDescriptor descriptor2)
        {
            return !(descriptor1 == descriptor2);
        }
    }
}
