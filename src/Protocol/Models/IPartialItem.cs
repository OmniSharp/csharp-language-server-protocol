using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IPartialItem<T>
    {
        /// <summary>
        /// An optional token that a server can use to report partial results (e.g. streaming) to
        /// the client.
        /// </summary>
        [Optional]
        ProgressToken? PartialResultToken { get; set; }
    }
    public interface IPartialItemRequest<out TResponse, T> : IRequest<TResponse>, IPartialItem<T> { }
}
