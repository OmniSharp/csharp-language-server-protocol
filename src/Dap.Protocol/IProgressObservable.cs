using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IProgressObservable : IObservable<ProgressEvent>
    {
        ProgressToken ProgressToken { get; }
    }
}
