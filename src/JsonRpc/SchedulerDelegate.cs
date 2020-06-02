using System;
using System.Reactive;
using System.Reactive.Concurrency;

namespace OmniSharp.Extensions.JsonRpc
{
    delegate IObservable<Unit> SchedulerDelegate(IObservable<Unit> contentModifiedToken, IScheduler scheduler);
}
