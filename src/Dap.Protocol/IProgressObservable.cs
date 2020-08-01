using System;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public interface IProgressObservable : IObservable<ProgressEvent>
    {
    }
}