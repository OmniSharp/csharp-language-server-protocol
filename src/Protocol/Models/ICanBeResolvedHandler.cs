using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Common interface for types that support resolution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICanBeResolvedHandler<T> : IJsonRpcRequestHandler<T, T>, ICanBeResolvedHandler
        where T : ICanBeResolved, IRequest<T>
    {
    }

    public interface ICanBeResolvedHandler
    {
    }
}
