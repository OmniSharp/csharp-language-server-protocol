using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc.Server;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestRouter : RequestRouterBase<IHandlerDescriptor>
    {
        private readonly HandlerCollection _collection;


        public RequestRouter(HandlerCollection collection, ISerializer serializer, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
            : base(serializer, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger<RequestRouter>())
        {
            _collection = collection;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        private IRequestDescriptor<IHandlerDescriptor> FindDescriptor(IMethodWithParams instance)
        {
            return new RequestDescriptor<IHandlerDescriptor>(_collection.Where(x => x.Method == instance.Method));
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptor(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptor(Request request)
        {
            return FindDescriptor(request);
        }
    }
}
