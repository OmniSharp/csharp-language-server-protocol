using System;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public interface IClientProgressManager
    {
        IObservable<IProgressObservable> Progress { get; }
    }
}