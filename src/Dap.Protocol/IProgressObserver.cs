using System;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IProgressObserver : IObserver<ProgressUpdateEvent>, IDisposable
    {
        ProgressToken ProgressId { get; }
        void OnNext(string message, double? percentage);
    }
}
