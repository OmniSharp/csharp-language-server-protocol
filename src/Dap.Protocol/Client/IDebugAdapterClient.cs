namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public interface IDebugAdapterClient : IDebugAdapterClientFacade, IDisposable
    {
        Task Initialize(CancellationToken token);
    }
}
