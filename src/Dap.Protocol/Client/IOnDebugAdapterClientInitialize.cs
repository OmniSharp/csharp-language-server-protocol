using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeRequestArguments"/> before it is sent to the server
    /// </summary>
    public interface IOnDebugAdapterClientInitialize : IEventingHandler
    {
        Task OnInitialize(IDebugAdapterClient client, InitializeRequestArguments request, CancellationToken cancellationToken);
    }
}
