using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(GeneralNames.SetTrace, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Server"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IClientLanguageClient), typeof(ILanguageClient))]
        public record SetTraceParams : IRequest<Unit>
        {
            /// <summary>
            /// The new value that should be assigned to the trace setting.
            /// </summary>
            public InitializeTrace Value { get; init; }
        }
    }

    namespace Server
    {
    }
}
