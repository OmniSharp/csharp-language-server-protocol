using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IPartialItem<T> : IPartialResultParams { }

    public interface IPartialItemRequest<out TResponse, T> : IRequest<TResponse>, IPartialItem<T>
    {
    }
}
