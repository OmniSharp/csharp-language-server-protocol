using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    internal class DebugAdapterRequestRouter : RequestRouterBase<IHandlerDescriptor>
    {
        private readonly DebugAdapterHandlerCollection _collection;


        public DebugAdapterRequestRouter(DebugAdapterHandlerCollection collection, ISerializer serializer, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
            : base(serializer, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger<RequestRouter>())
        {
            _collection = collection;
        }

        public IDisposable Add(IJsonRpcHandler handler)
        {
            return _collection.Add(handler);
        }

        private List<IHandlerDescriptor> FindDescriptor(IMethodWithParams instance)
        {
            return _collection.Where(x => x.Method == instance.Method).ToList();
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Notification notification)
        {
            var descriptors = FindDescriptor(notification);
            if (descriptors.Count == 0) return new RequestDescriptor<IHandlerDescriptor>(null, Array.Empty<IHandlerDescriptor>());
            var paramsValue = DeserializeParams(descriptors[0], notification.Params);
            return new RequestDescriptor<IHandlerDescriptor>(paramsValue, descriptors);
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Request request)
        {
            var descriptors = FindDescriptor(request);
            if (descriptors.Count == 0) return new RequestDescriptor<IHandlerDescriptor>(null, Array.Empty<IHandlerDescriptor>());
            var paramsValue = DeserializeParams(descriptors[0], request.Params);
            return new RequestDescriptor<IHandlerDescriptor>(paramsValue, descriptors);
        }
    }
}
