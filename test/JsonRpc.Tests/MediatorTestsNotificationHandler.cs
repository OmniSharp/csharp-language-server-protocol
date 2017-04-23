using System;
using System.Reflection;
using System.Threading.Tasks;
using JsonRpc.Server;
using NSubstitute;
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
            var mediator = new RequestRouter(collection);

            var notification = new Notification("exit", null);

            mediator.RouteNotification(notification);

            await exitHandler.Received(1).Handle();
        }

    }
}