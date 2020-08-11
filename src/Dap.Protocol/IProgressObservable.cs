using System;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IProgressObservable : IObservable<ProgressEvent>
    {
    }
}
