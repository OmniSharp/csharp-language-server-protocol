using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeRequestArguments" /> before it is processed by the server
    /// </summary>
    public delegate Task OnDebugAdapterServerInitializeDelegate(IDebugAdapterServer server, InitializeRequestArguments request, CancellationToken cancellationToken);
}
