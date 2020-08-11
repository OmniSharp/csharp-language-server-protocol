using System;
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

    public interface ICanBeIdentifiedHandler
    {
        /// <summary>
        /// An id that that determines if a handler is unique or not for purposes of routing requests
        /// </summary>
        /// <remarks>
        /// Some requests can "fan out" to multiple handlers to pull back data for the same document selector
        /// </remarks>
        Guid Id { get; }
    }
}
