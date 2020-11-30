using System.Collections.Generic;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IPartialItems<T> : IPartialResultParams { }

    public interface IPartialItemsRequest<out TResponse, T> : IRequest<TResponse>, IPartialItems<T>
        where TResponse : IEnumerable<T>?
    {
    }
}
