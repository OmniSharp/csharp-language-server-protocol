using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    [DebuggerDisplay("{ToString()}")]
    internal class HandlerTypeDescriptor : IHandlerTypeDescriptor, IEquatable<HandlerTypeDescriptor>
    {
        public HandlerTypeDescriptor(Type handlerType)
        {
            var method = MethodAttribute.From(handlerType)!;
            Method = method.Method;
            Direction = method.Direction;
            if (handlerType.IsGenericTypeDefinition && handlerType.IsPublic)
            {
                var parameter = handlerType.GetTypeInfo().GenericTypeParameters[0];
                var constraints = parameter.GetGenericParameterConstraints();
                if (constraints.Length == 1)
                {
                    handlerType = handlerType.MakeGenericType(handlerType.GetTypeInfo().GenericTypeParameters[0].GetGenericParameterConstraints()[0]);
                }
            }

            HandlerType = handlerType;
            InterfaceType = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);

            // This allows for us to have derived types
            // We are making the assumption that interface given here
            // if a GTD will have a constraint on the first generic type parameter
            // that is the real base type for this interface.
            if (InterfaceType.IsGenericType)
            {
                ParamsType = InterfaceType.GetGenericArguments()[0];
            }

            HasParamsType = ParamsType != null;
            IsNotification = handlerType
                            .GetInterfaces()
                            .Any(z => z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z.GetGenericTypeDefinition()));
            IsRequest = !IsNotification;

            var requestInterface = ParamsType?
                                  .GetInterfaces()
                                  .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                ResponseType = requestInterface.GetGenericArguments()[0];
            HasResponseType = ResponseType != null && ResponseType != typeof(Unit);

            var processAttributes = HandlerType
                                   .GetCustomAttributes(true)
                                   .Concat(HandlerType.GetCustomAttributes(true))
                                   .Concat(InterfaceType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                                   .Concat(HandlerType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                                   .OfType<ProcessAttribute>()
                                   .ToArray();
            RequestProcessType = processAttributes
                                .FirstOrDefault()?.Type;
        }

        public string Method { get; }
        public Direction Direction { get; }
        public RequestProcessType? RequestProcessType { get; }
        public bool IsRequest { get; }
        public Type HandlerType { get; }
        public Type InterfaceType { get; }
        public bool IsNotification { get; }
        public bool HasParamsType { get; }
        public Type? ParamsType { get; }
        public bool HasResponseType { get; }
        public Type? ResponseType { get; }
        public override string ToString() => $"{Method}:{HandlerType.FullName}";

        public bool Equals(HandlerTypeDescriptor? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Method == other.Method && HandlerType == other.HandlerType && InterfaceType == other.InterfaceType;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HandlerTypeDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Method.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ HandlerType.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ InterfaceType.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HandlerTypeDescriptor left, HandlerTypeDescriptor right) => Equals(left, right);

        public static bool operator !=(HandlerTypeDescriptor left, HandlerTypeDescriptor right) => !Equals(left, right);
    }
}
