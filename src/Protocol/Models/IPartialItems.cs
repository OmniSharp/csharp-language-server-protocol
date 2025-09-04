using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IPartialItems<T> : IPartialResultParams { }
    public interface IPartialItemsWithInitialValue<out TInitial, T> : IPartialResultParams { }

    public interface IPartialItemsRequest<out TResponse, T> : IRequest<TResponse>, IPartialItems<T>
        where TResponse : IEnumerable<T>?
    {
    }

    public interface IPartialItemsWithInitialValueRequest<out TResponse, T> : IRequest<TResponse>, IPartialItemsWithInitialValue<TResponse, T>
        where TResponse : IEnumerable<T>?
    {
    }
}
