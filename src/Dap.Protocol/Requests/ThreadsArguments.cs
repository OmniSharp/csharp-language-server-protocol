using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Threads, Direction.ClientToServer)]
    public class ThreadsArguments : IRequest<ThreadsResponse>
    {
    }

}
