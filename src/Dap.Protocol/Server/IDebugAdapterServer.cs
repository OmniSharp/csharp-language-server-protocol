namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public interface IDebugAdapterServer : IDebugAdapterServerFacade, IDisposable
    {
        Task Initialize(CancellationToken token);
    }
}
