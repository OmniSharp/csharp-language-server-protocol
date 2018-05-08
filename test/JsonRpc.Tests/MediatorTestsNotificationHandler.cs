using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;

namespace JsonRpc.Tests
{
    public class MediatorTestsNotificationHandler
    {
        [Method("exit")]
        public interface IExitHandler : IJsonRpcNotificationHandler { }

        [Fact]
        public async Task ExecutesHandler()
        {
            var exitHandler = Substitute.For<IExitHandler>();
            var mediator = Substitute.For<IMediator>();

            var collection = new HandlerCollection { exitHandler };
            IRequestRouter router = new RequestRouter(collection, new Serializer(), mediator);

            var notification = new Notification("exit", null);

            await router.RouteNotification(notification);

            await exitHandler.Received(1).Handle(null, CancellationToken.None);
        }

    }
}
