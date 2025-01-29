using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(GeneralNames.LogTrace, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Client")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))]
        public record LogTraceParams : IRequest<Unit>
        {
            /// <summary>
            /// The message to be logged.
            /// </summary>
            public string Message { get; init; } = null!;

            /// <summary>
            /// Additional information that can be computed if the `trace` configuration is set to `'verbose'`
            /// </summary>
            [Optional]
            public string? Verbose { get; init; }
        }
    }
}
