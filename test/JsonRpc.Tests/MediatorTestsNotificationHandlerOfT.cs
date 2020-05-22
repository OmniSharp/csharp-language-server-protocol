using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class MediatorTestsNotificationHandlerOfT : AutoTestBase
    {
        [Method("$/cancelRequest")]
        public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }

        public class CancelParams : IRequest
        {
            public object Id { get; set; }
        }

        public MediatorTestsNotificationHandlerOfT(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Services
                .AddJsonRpcMediatR(new[] { typeof(MediatorTestsNotificationHandler).Assembly })
                .AddSingleton<ISerializer>(new JsonRpcSerializer());
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var cancelRequestHandler = Substitute.For<ICancelRequestHandler>();
            var mediator = Substitute.For<IMediator>();

            var collection = new HandlerCollection { cancelRequestHandler };
            AutoSubstitute.Provide(collection);
            var router = AutoSubstitute.Resolve<RequestRouter>();

            var @params = new CancelParams() { Id = Guid.NewGuid() };
            var notification = new Notification("$/cancelRequest", JObject.Parse(JsonConvert.SerializeObject(@params)));

            await router.RouteNotification(router.GetDescriptor(notification), notification, CancellationToken.None);

            await cancelRequestHandler.Received(1).Handle(Arg.Any<CancelParams>(), Arg.Any<CancellationToken>());
        }

    }
}
