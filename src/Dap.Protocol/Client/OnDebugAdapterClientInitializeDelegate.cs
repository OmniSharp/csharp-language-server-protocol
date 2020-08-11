using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeRequestArguments" /> before it is sent to the server
    /// </summary>
    public delegate Task OnDebugAdapterClientInitializeDelegate(IDebugAdapterClient client, InitializeRequestArguments request, CancellationToken cancellationToken);
}
