using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeRequestArguments" /> before it is processed by the server
    /// </summary>
    public interface IOnDebugAdapterServerInitialize : IEventingHandler
    {
        Task OnInitialize(IDebugAdapterServer server, InitializeRequestArguments request, CancellationToken cancellationToken);
    }
}
