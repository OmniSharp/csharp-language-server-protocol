using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    internal class LanguageProtocolEventingHelper
    {
        public static Task Run<TDelegate, THandler>(
            IEnumerable<TDelegate> delegates,
            Func<TDelegate, CancellationToken, Task> executeDelegate,
            IEnumerable<THandler> handlers,
            Func<THandler, CancellationToken, Task> executeHandler,
            int? concurrency,
            IScheduler scheduler,
            CancellationToken cancellationToken
        )
        {
            var events = delegates.Select(z => Observable.FromAsync(ct => executeDelegate(z, ct)))
                                  .Concat(handlers.Select(z => Observable.FromAsync(ct => executeHandler(z, ct))))
                                  .ToObservable();


            if (concurrency.HasValue)
            {
                return events.Merge(concurrency.Value)
                             .LastOrDefaultAsync()
                             .ToTask(cancellationToken, scheduler);
            }

            return events
                  .Merge()
                  .LastOrDefaultAsync()
                  .ToTask(cancellationToken, scheduler);
        }
    }
}
