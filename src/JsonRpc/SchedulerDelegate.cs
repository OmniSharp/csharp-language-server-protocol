using System;
using System.Reactive;

namespace OmniSharp.Extensions.JsonRpc
{
    delegate IObservable<Unit> SchedulerDelegate(IObservable<Unit> contentModifiedToken);
}