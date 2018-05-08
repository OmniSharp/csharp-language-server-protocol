using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;

namespace JsonRpc.Tests
{
    public class MediatorTestsNotificationHandlerOfT
    {
        [Method("$/cancelRequest")]
        public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }

        public class CancelParams : IRequest
        {
            public object Id { get; set; }
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var cancelRequestHandler = Substitute.For<ICancelRequestHandler>();
            var mediator = Substitute.For<IMediator>();

            var collection = new HandlerCollection { cancelRequestHandler };
            IRequestRouter router = new RequestRouter(collection, new Serializer(), mediator);

            var @params = new CancelParams() { Id = Guid.NewGuid() };
            var notification = new Notification("$/cancelRequest", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await router.RouteNotification(notification);

            await cancelRequestHandler.Received(1).Handle(Arg.Any<CancelParams>(), Arg.Any<CancellationToken>());
        }

    }
}
