using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.WorkDoneProgressCreate, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface IWorkDoneProgressCreateHandler : IJsonRpcRequestHandler<WorkDoneProgressCreateParams> { }

    public abstract class WorkDoneProgressCreateHandler : IWorkDoneProgressCreateHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken);
    }
}
