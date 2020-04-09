using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IScheduler : IDisposable
    {
        void Start();
        void Add(RequestProcessType type, string name, IObservable<Unit> request);
    }

    public static class SchedulerExtensions
    {
        public static void Add(this IScheduler scheduler, RequestProcessType type, string name, Func<Task> request)
        {
            scheduler.Add(type, name, Observable.FromAsync(request));
        }
    }
}
