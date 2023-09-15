using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IPartialItem<T> : IPartialResultParams { }
    public interface IPartialItemWithInitialValue<T, out TInitial> : IPartialResultParams { }

    public interface IPartialItemRequest<out TResponse, T> : IRequest<TResponse>, IPartialItem<T>
    {
    }

    public interface IPartialItemWithInitialValueRequest<out TResponse, T> : IRequest<TResponse>, IPartialItemWithInitialValue<T, TResponse>
        where TResponse : T
    {
    }
}
