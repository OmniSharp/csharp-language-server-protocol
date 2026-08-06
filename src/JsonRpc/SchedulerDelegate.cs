using System;
using System.Reactive.Concurrency;
using ReactiveUnit = System.Reactive.Unit;

namespace OmniSharp.Extensions.JsonRpc
{
    internal delegate IObservable<ReactiveUnit> SchedulerDelegate(IObservable<ReactiveUnit> contentModifiedToken, IScheduler scheduler);
}
