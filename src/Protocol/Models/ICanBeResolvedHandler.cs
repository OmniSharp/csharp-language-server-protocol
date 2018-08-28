using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Common interface for types that support resolution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICanBeResolvedHandler<T> : IJsonRpcRequestHandler<T, T>
        where T : ICanBeResolved, IRequest<T>
    {
        /// <summary>
        /// Method that determines if a handler can be used to resolve this one
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanResolve(T value);
    }
}
