using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Request = OmniSharp.Extensions.JsonRpc.Server.Request;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    internal class DebugAdapterRequestRouter : RequestRouterBase<IHandlerDescriptor>
    {
        private readonly DebugAdapterHandlerCollection _collection;

        public DebugAdapterRequestRouter(DebugAdapterHandlerCollection collection, ISerializer serializer, IServiceScopeFactory serviceScopeFactory, ILogger<DebugAdapterRequestRouter> logger)
            : base(serializer, serviceScopeFactory, logger)
        {
            _collection = collection;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        private IRequestDescriptor<IHandlerDescriptor> FindDescriptor(IMethodWithParams instance)
        {
            return new RequestDescriptor<IHandlerDescriptor>( _collection.Where(x => x.Method == instance.Method));
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Request request)
        {
            return FindDescriptor(request);
        }
    }
}
