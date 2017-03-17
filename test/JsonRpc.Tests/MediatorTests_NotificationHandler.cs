using System;
using System.Reflection;
using System.Threading.Tasks;
using JsonRPC;
using JsonRPC.Server;
using NSubstitute;
using Xunit;

namespace Lsp.Tests
{
    public class MediatorTests_NotificationHandler
    {
        [Method("exit")]
        interface IExitHandler : INotificationHandler { }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var notification = new Notification("$/cancelRequest", null);

            mediator.HandleNotification(notification);
        }

    }
}