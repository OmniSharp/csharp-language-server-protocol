using System;
using System.Threading;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public interface IServerProgressManager
    {
        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent> onError = null, Func<ProgressEndEvent> onComplete = null);
    }
}