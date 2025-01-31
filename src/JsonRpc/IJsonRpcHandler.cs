using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    /// A simple marker interface to use for storing handling's (which will be cast out later)
    /// </summary>
    public interface IJsonRpcHandler
    {
    }

    /// <summary>
    ///  Marker interface for source generation to properly know that this IRequest is a real request and not a notification
    /// </summary>
    public interface IJsonRpcRequest : IRequest<Unit>
    {

    }
}
