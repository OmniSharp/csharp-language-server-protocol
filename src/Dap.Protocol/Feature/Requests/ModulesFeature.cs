using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Modules, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ModulesArguments : IRequest<ModulesResponse>
        {
            /// <summary>
            /// The index of the first module to return; if omitted modules start at 0.
            /// </summary>
            [Optional]
            public long? StartModule { get; init; }

            /// <summary>
            /// The number of modules to return. If moduleCount is not specified or 0, all modules are returned.
            /// </summary>
            [Optional]
            public long? ModuleCount { get; init; }
        }

        public record ModulesResponse
        {
            /// <summary>
            /// All modules or range of modules.
            /// </summary>
            public Container<Module> Modules { get; init; }

            /// <summary>
            /// The total number of modules available.
            /// </summary>
            [Optional]
            public long? TotalModules { get; init; }
        }
    }

    namespace Models
    {
    }
}
