using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.WorkDoneProgressCreate, Direction.ServerToClient)]
    public interface IWorkDoneProgressCreateHandler : IJsonRpcRequestHandler<WorkDoneProgressCreateParams> { }

    public abstract class WorkDoneProgressCreateHandler : IWorkDoneProgressCreateHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken);
    }

    public static class WorkDoneProgressCreateExtensions
    {
public static ILanguageClientRegistry OnWorkDoneProgressCreate(this ILanguageClientRegistry registry,
            Func<WorkDoneProgressCreateParams, CancellationToken, Task>
                handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCreate, RequestHandler.For(handler));
        }

public static ILanguageClientRegistry OnWorkDoneProgressCreate(this ILanguageClientRegistry registry,
            Func<WorkDoneProgressCreateParams, Task>
                handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCreate, RequestHandler.For(handler));
        }

        public static async Task RequestWorkDoneProgressCreate(this IWindowLanguageServer mediator, WorkDoneProgressCreateParams @params, CancellationToken cancellationToken = default)
        {
            await mediator.SendRequest(@params, cancellationToken);
        }
    }
}
