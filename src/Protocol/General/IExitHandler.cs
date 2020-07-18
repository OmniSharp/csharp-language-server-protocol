using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }

    public abstract class ExitHandler : IExitHandler
    {
        public virtual async Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            await Handle(cancellationToken);
            return Unit.Value;
        }

        protected abstract Task Handle(CancellationToken cancellationToken);
    }

    public static partial class ExitExtensions
    {
        public static void SendExit(this ILanguageClient mediator)
        {
            mediator.SendNotification(ExitParams.Instance);
        }
    }
}
