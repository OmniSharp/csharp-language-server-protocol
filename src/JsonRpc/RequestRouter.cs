using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class RequestRouter : RequestRouterBase<IHandlerDescriptor>
    {
        private readonly IHandlersManager _collection;

        public RequestRouter(
            IHandlersManager collection,
            ISerializer serializer,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RequestRouter> logger,
            IActivityTracingStrategy? activityTracingStrategy = null
        )
            : base(serializer, serviceScopeFactory, logger, activityTracingStrategy) =>
            _collection = collection;

        private IHandlerDescriptor FindDescriptor(IMethodWithParams instance) => _collection.Descriptors.FirstOrDefault(x => x.Method == instance.Method);

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Notification notification) => new RequestDescriptor<IHandlerDescriptor>(FindDescriptor(notification));

        public override IRequestDescriptor<IHandlerDescriptor> GetDescriptors(Request request) => new RequestDescriptor<IHandlerDescriptor>(FindDescriptor(request));
    }
}
