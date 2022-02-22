using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.LoadedSources, Direction.ClientToServer)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record LoadedSourcesArguments : IRequest<LoadedSourcesResponse>;

        public record LoadedSourcesResponse
        {
            /// <summary>
            /// Set of loaded sources.
            /// </summary>
            public Container<Source> Sources { get; init; } = null!;
        }
    }

    namespace Models
    {
    }
}
