namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public interface IDebugAdapterClientFacade : IDebugAdapterClientProxy
    {
        IDebugAdapterClientProgressManager ProgressManager { get; }
    }
}