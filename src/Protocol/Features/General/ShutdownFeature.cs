using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Serial]
        [Method(GeneralNames.Shutdown, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.General"), GenerateHandlerMethods, GenerateRequestMethods(typeof(ILanguageClient))]
        public partial record ShutdownParams : IRequest<Unit>
        {
            public static ShutdownParams Instance { get; } = new();
        }
    }

    namespace General
    {
        public static partial class ShutdownExtensions
        {
            public static Task RequestShutdown(this ILanguageClient mediator, CancellationToken cancellationToken = default) =>
                mediator.SendRequest(ShutdownParams.Instance, cancellationToken);
        }
    }
}
