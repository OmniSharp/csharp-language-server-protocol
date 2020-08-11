using System;
using System.Reactive;
using System.Reactive.Concurrency;

namespace OmniSharp.Extensions.JsonRpc
{
    internal delegate IObservable<Unit> SchedulerDelegate(IObservable<Unit> contentModifiedToken, IScheduler scheduler);
}
