using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
        IServiceProvider ServiceProvider { get; }
    }

    public interface IRequestDescriptor<out TDescriptor> : IEnumerable<TDescriptor>
    {
        TDescriptor Default { get; }
    }

    class RequestDescriptor<TDescriptor> : IRequestDescriptor<TDescriptor>
    {
        private IEnumerable<TDescriptor> _descriptors;

        public RequestDescriptor(IEnumerable<TDescriptor> descriptors)
        {
            var enumerable = descriptors as TDescriptor[] ?? descriptors.ToArray();
            _descriptors = enumerable;
            Default = enumerable.FirstOrDefault();
        }

        public IEnumerator<TDescriptor> GetEnumerator() => _descriptors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _descriptors).GetEnumerator();

        public TDescriptor Default { get; }
    }

    public interface IRequestRouter<TDescriptor> : IRequestRouter
    {
        IRequestDescriptor<TDescriptor> GetDescriptor(Notification notification);
        IRequestDescriptor<TDescriptor> GetDescriptor(Request request);
        Task RouteNotification(IRequestDescriptor<TDescriptor> descriptors, Notification notification, CancellationToken token);
        Task<ErrorResponse> RouteRequest(IRequestDescriptor<TDescriptor> descriptors, Request request, CancellationToken token);
    }

    interface IAggregateResults
    {
        object AggregateResults(IEnumerable<object> values);
    }
}
