using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    [Serial]
    [Method(GeneralNames.Shutdown, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IShutdownHandler : IJsonRpcRequestHandler<ShutdownParams>
    {
    }

    public abstract class ShutdownHandler : IShutdownHandler
    {
        public virtual async Task<Unit> Handle(ShutdownParams request, CancellationToken cancellationToken)
        {
            await Handle(cancellationToken);
            return Unit.Value;
        }

        protected abstract Task Handle(CancellationToken cancellationToken);
    }

    public static partial class ShutdownExtensions
    {
        public static Task RequestShutdown(this ILanguageClient mediator, CancellationToken cancellationToken = default) =>
            mediator.SendRequest(ShutdownParams.Instance, cancellationToken);
    }
}
