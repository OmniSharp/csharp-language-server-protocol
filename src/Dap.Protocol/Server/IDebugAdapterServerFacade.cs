namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public interface IDebugAdapterServerFacade : IDebugAdapterServerProxy
    {
        IDebugAdapterServerProgressManager ProgressManager { get; }
    }
}