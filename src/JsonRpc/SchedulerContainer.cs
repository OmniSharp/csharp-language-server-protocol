using System.Reactive.Concurrency;

namespace OmniSharp.Extensions.JsonRpc
{
    internal record SchedulerContainer(IScheduler Scheduler);
}
