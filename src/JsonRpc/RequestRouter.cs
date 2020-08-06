using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc.Server;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class RequestRouter : RequestRouterBase<IHandlerDescriptor>
    {
        private readonly IHandlersManager _collection;

        public RequestRouter(
            IHandlersManager collection,
            ISerializer serializer,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RequestRouter> logger
        )
            : base(serializer, serviceScopeFactory, logger)
        {
            _collection = collection;
        }

        private IHandlerDescriptor FindDescriptor(IMethodWithParams instance)
        {
            return _collection.Descriptors.FirstOrDefault(x => x.Method == instance.Method);
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Notification notification)
        {
            return new RequestDescriptor<IHandlerDescriptor>(FindDescriptor(notification));
        }

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Request request)
        {
            return new RequestDescriptor<IHandlerDescriptor>(FindDescriptor(request));
        }
    }
}
