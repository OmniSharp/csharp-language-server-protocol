namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public interface IDebugAdapterClientProgressManager
    {
        IObservable<IProgressObservable> Progress { get; }
    }
}
