using System;
using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using Xunit;

namespace JsonRpc.Tests
{
    public class MediatorTestsNotificationHandler
    {
        [Method("exit")]
        public interface IExitHandler : INotificationHandler { }

        [Fact]
        public async Task ExecutesHandler()
        {
            var exitHandler = Substitute.For<IExitHandler>();

            var collection = new HandlerCollection { exitHandler };
            IRequestRouter mediator = new RequestRouter(collection);

            var notification = new Notification("exit", null);

            await mediator.RouteNotification(notification);

            await exitHandler.Received(1).Handle();
        }

    }
}
