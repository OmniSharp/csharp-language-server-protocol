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


        public RequestRouter(HandlerCollection collection, ISerializer serializer, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
            : base(serializer, serviceScopeFactory, loggerFactory.CreateLogger<RequestRouter>())
        {
            _collection = collection;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        private IHandlerDescriptor FindDescriptor(IMethodWithParams instance)
        {
            return _collection.FirstOrDefault(x => x.Method == instance.Method);
        }

        public override IHandlerDescriptor GetDescriptor(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public override IHandlerDescriptor GetDescriptor(Request request)
        {
            return FindDescriptor(request);
        }
    }
}
