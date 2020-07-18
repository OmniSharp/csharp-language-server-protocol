using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.ShowMessage, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface IShowMessageHandler : IJsonRpcNotificationHandler<ShowMessageParams> { }

    public abstract class ShowMessageHandler : IShowMessageHandler
    {
        public abstract Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken);
    }
}
