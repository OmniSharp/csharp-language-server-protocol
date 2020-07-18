using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Serial, Method(WindowNames.ShowMessageRequest, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface IShowMessageRequestHandler : IJsonRpcRequestHandler<ShowMessageRequestParams, MessageActionItem> { }

    public abstract class ShowMessageRequestHandler : IShowMessageRequestHandler
    {
        public abstract Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken);
    }
}
