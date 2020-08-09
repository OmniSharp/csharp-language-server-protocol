using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    internal class DebugAdapterEventingHelper
    {
        public static Task Run<TDelegate, THandler>(
            IEnumerable<TDelegate> delegates,
            Func<TDelegate, CancellationToken, Task> executeDelegate,
            IEnumerable<THandler> handlers,
            Func<THandler, CancellationToken, Task> executeHandler,
            int? concurrency,
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
                    .ToTask(cancellationToken);
            }

            return events
                .Merge()
                .LastOrDefaultAsync()
                .ToTask(cancellationToken);
        }
    }
}
