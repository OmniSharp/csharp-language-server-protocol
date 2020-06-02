using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Common interface for types that support resolution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICanBeResolvedHandler<T, TData> : IJsonRpcRequestHandler<T, T>, ICanBeIdentifiedHandler
        where T : ICanBeResolved<TData>, IRequest<T>
        where TData : CanBeResolvedData
    {

    }

    public interface ICanBeIdentifiedHandler
    {
        Guid Id { get; }
    }
}
